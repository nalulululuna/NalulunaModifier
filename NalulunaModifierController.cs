using BS_Utils.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Xft;

namespace NalulunaModifier
{
    public class NalulunaModifierController : MonoBehaviour
    {
        enum AvatarType { None, VMCAvatar, CustomAvatar }

        public static NalulunaModifierController instance { get; private set; }

        private AudioTimeSyncController _audioTimeSyncController;
        private AudioSource _audioSource;
        private PlayerController _playerController;
        private Transform _rightSaberTransform;
        private Transform _leftSaberTransform;
        private SaberClashChecker _saberClashChecker;
        private SaberBurnMarkArea _saberBurnMarkArea;
        private SaberBurnMarkSparkles _saberBurnMarkSparkles;
        private BeatmapObjectManager _beatmapObjectManager;
        private BeatmapObjectSpawnController _beatmapObjectSpawnController;
        private GameEnergyCounter _gameEnergyCounter;
        private GamePause _gamePause;
        private Transform _footL;
        private Transform _footR;
        private AvatarType _avatarType;
        private GameObject _feetSaber;

        private Saber _saberFootL;
        private Saber _saberFootR;
        private Saber _saberFootL2;
        private Saber _saberFootR2;
        private Saber _saberL2;
        private Saber _saberR2;
        private Saber _saberWaistL;
        private Saber _saberWaistR;
        private Saber _saberMouthL;
        private Saber _saberMouthR;
        private Saber _saberHeadL;
        private Saber _saberHeadR;
        private Transform _hips;
        private Transform _head;

        private InputDevice leftFootTracker = new InputDevice();
        private InputDevice rightFootTracker = new InputDevice();

        private bool _init;        

        private void FindTrackers()
        {
            try
            {
                var trackers = new List<(InputDevice device, float x, float y)>();

                List<InputDevice> allDevices = new List<InputDevice>();
                InputDevices.GetDevices(allDevices);
                foreach (InputDevice device in allDevices)
                {
                    Logger.log.Debug($"name={device.name}, manufacturer={device.manufacturer}, serial={device.serialNumber}, isValid={device.isValid}, role={device.role}, characteristics={device.characteristics}");

                    if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position) &&
                        device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
                    {
                        Logger.log.Debug($"  position=({position.x}, {position.y}, {position.z})");
                        Logger.log.Debug($"  rotation=({rotation.x}, {rotation.y}, {rotation.z})");

                        if ((device.role == InputDeviceRole.HardwareTracker) &&
                            (device.isValid) &&
                            (!(position.x == 0 && position.y == 0 && position.z == 0)))
                        {
                            trackers.Add((device, position.x, position.y));
                        }
                    }
                }

                if (trackers.Count >= 2)
                {
                    var ordered = trackers.OrderBy(tracker => tracker.y).ToArray();
                    if (ordered[0].x < ordered[1].x)
                    {
                        leftFootTracker = ordered[0].device;
                        rightFootTracker = ordered[1].device;
                    }
                    else
                    {
                        leftFootTracker = ordered[1].device;
                        rightFootTracker = ordered[0].device;
                    }

                    Logger.log.Debug($"leftFootTracker: " + $"name={leftFootTracker.name}, serial={leftFootTracker.serialNumber}");
                    Logger.log.Debug($"rightFootTracker: " + $"name={rightFootTracker.name}, serial={rightFootTracker.serialNumber}");
                }
            }
            catch
            {
                Logger.log.Debug($"Exception on FindTrackers");
            }
        }

        private void UpdateTrackers()
        {
            if (leftFootTracker.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftFoot) &&
                rightFootTracker.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightFoot))
            {
                if (leftFoot.x > rightFoot.x)
                {
                    InputDevice temp = leftFootTracker;
                    leftFootTracker = rightFootTracker;
                    rightFootTracker = temp;
                }

                Logger.log.Debug($"leftFootTracker: " + $"name={leftFootTracker.name}, manufacturer={leftFootTracker.manufacturer}, serial={leftFootTracker.serialNumber}");
                Logger.log.Debug($"rightFootTracker: " + $"name={rightFootTracker.name}, manufacturer={rightFootTracker.manufacturer}, serial={rightFootTracker.serialNumber}");
            }
        }

        private void Awake()
        {
            if (instance != null)
            {
                Logger.log?.Warn($"Instance of {this.GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            instance = this;
            Logger.log?.Debug($"{name}: Awake()");

            FindTrackers();
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            Config.watcher.Changed += OnConfigChanged;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            Config.watcher.Changed -= OnConfigChanged;
        }

        public void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            _init = false;

            if (newScene.name == "GameCore")
            {
                Config.Read();

                if (Config.parabola || Config.noBlue || Config.noRed || Config.redToBlue || Config.blueToRed || Config.centering ||
                    Config.feet || Config.noDirection || Config.flatNotes ||
                    Config.fourSabers || Config.topNotesToFeet || Config.middleNotesToFeet || Config.bottomNotesToFeet ||
                    Config.superhot || Config.vacuum || Config.ninjaMaster)
                {
                    ScoreSubmission.DisableSubmission(Plugin.Name);
                }

                if (Config.feet || Config.feetAvatar || Config.feetTracker || Config.fourSabers)
                {
                    if (_feetSaber == null)
                    {
                        _feetSaber = new GameObject("FeetSaber");
                    }
                }

                if (Config.feetTracker)
                {
                    FindTrackers();
                }

                StartCoroutine(OnGameCoreCoroutine());
            }
            else
            {
                OnNotGameCore();
            }
        }

        private void OnConfigChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Logger.log.Debug("OnConfigChanged");
            Config.ReadFeetPosRot();
        }

        public void OnPause()
        {
            if (_saberFootL != null) _saberFootL.gameObject.SetActive(false);
            if (_saberFootR != null) _saberFootR.gameObject.SetActive(false);
            if (_saberFootL2 != null) _saberFootL2.gameObject.SetActive(false);
            if (_saberFootR2 != null) _saberFootR2.gameObject.SetActive(false);
            if (_saberL2 != null) _saberL2.gameObject.SetActive(false);
            if (_saberR2 != null) _saberR2.gameObject.SetActive(false);
            if (_saberWaistL != null) _saberWaistL.gameObject.SetActive(false);
            if (_saberWaistR != null) _saberWaistR.gameObject.SetActive(false);
            if (_saberMouthL != null) _saberMouthL.gameObject.SetActive(false);
            if (_saberMouthR != null) _saberMouthR.gameObject.SetActive(false);
            if (_saberHeadL != null) _saberHeadL.gameObject.SetActive(false);
            if (_saberHeadR != null) _saberHeadR.gameObject.SetActive(false);
        }

        public void OnPauseResume()
        {
            UpdateSaberActive();

            if (_saberFootL != null) _saberFootL.gameObject.SetActive(true);
            if (_saberFootR != null) _saberFootR.gameObject.SetActive(true);
            if (_saberFootL2 != null) _saberFootL2.gameObject.SetActive(true);
            if (_saberFootR2 != null) _saberFootR2.gameObject.SetActive(true);
            if (_saberL2 != null) _saberL2.gameObject.SetActive(true);
            if (_saberR2 != null) _saberR2.gameObject.SetActive(true);
            if (_saberWaistL != null) _saberWaistL.gameObject.SetActive(true);
            if (_saberWaistR != null) _saberWaistR.gameObject.SetActive(true);
            if (_saberMouthL != null) _saberMouthL.gameObject.SetActive(true);
            if (_saberMouthR != null) _saberMouthR.gameObject.SetActive(true);
            if (_saberHeadL != null) _saberHeadL.gameObject.SetActive(true);
            if (_saberHeadR != null) _saberHeadR.gameObject.SetActive(true);
        }

        private void UpdateSaberActive()
        {
            if (!(Config.blueToRed || !(Config.noRed || Config.redToBlue)))
            {
                _playerController.leftSaber.gameObject.SetActive(false);
            }

            if (!(Config.redToBlue || !(Config.noBlue || Config.blueToRed)))
            {
                _playerController.rightSaber.gameObject.SetActive(false);
            }

            if (Config.vacuum && !Config.ninjaMaster)
            {
                _playerController.leftSaber.gameObject.SetActive(false);
            }
        }

        private void EnumChildren(GameObject parent, string indent)
        {
            Logger.log.Debug(indent + parent.name);
            foreach (Transform transform in parent.GetComponentInChildren<Transform>())
            {
                EnumChildren(transform.gameObject, indent + "  ");
            }
        }

        private IEnumerator EnumAllObjectsCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.6f);
            foreach (GameObject g in Array.FindAll(FindObjectsOfType<GameObject>(), x => x.transform.parent == null))
            {
                EnumChildren(g, "");
                yield return null;
            }
        }

        private GameObject FindChildren(GameObject parent, string name, bool includeInactive = false)
        {
            var children = parent.GetComponentsInChildren<Transform>(includeInactive);
            foreach (var transform in children)
            {
                if (transform.name == name)
                {
                    return transform.gameObject;
                }
            }
            return null;
        }

        private IEnumerator OnGameCoreCoroutine()
        {
            if (_audioTimeSyncController == null)
                _audioTimeSyncController = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
            _audioSource = _audioTimeSyncController.GetPrivateField<AudioSource>("_audioSource");

            if (_gamePause == null)
                _gamePause = Resources.FindObjectsOfTypeAll<GamePause>().FirstOrDefault();
            _gamePause.didPauseEvent += OnPause;
            _gamePause.didResumeEvent += OnPauseResume;

            // wait for CustomSaber mod
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());
            yield return new WaitForSecondsRealtime(0.1f);

            if (_playerController == null)
                _playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
            if (_rightSaberTransform == null)
                _rightSaberTransform = _playerController.rightSaber.transform;
            if (_leftSaberTransform == null)
                _leftSaberTransform = _playerController.leftSaber.transform;
            if (_saberClashChecker == null)
                _saberClashChecker = Resources.FindObjectsOfTypeAll<SaberClashChecker>().FirstOrDefault();
            if (_saberBurnMarkArea == null)
                _saberBurnMarkArea = Resources.FindObjectsOfTypeAll<SaberBurnMarkArea>().FirstOrDefault();
            if (_saberBurnMarkSparkles == null)
                _saberBurnMarkSparkles = Resources.FindObjectsOfTypeAll<SaberBurnMarkSparkles>().FirstOrDefault();

            // need some wait to GetNoteOffset
            if (Config.centering)
            {
                if (_beatmapObjectSpawnController == null)
                    _beatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault();
                BeatmapObjectSpawnMovementData beatmapObjectSpawnMovementData = _beatmapObjectSpawnController.GetPrivateField<BeatmapObjectSpawnMovementData>("_beatmapObjectSpawnMovementData");
                Vector3 leftBase = beatmapObjectSpawnMovementData.GetNoteOffset(0, NoteLineLayer.Base);
                Vector3 rightTop = beatmapObjectSpawnMovementData.GetNoteOffset(3, NoteLineLayer.Top);
                NoteJumpManualUpdate.center = (leftBase + rightTop) / 2;
                //Logger.log.Debug($"leftBase={leftBase.x}, {leftBase.y}, {leftBase.z}");
                //Logger.log.Debug($"rightTop={rightTop.x}, {rightTop.y}, {rightTop.z}");
            }

            if (Config.hideSabers)
            {
                SetSaberVisible(_playerController.rightSaber, false);
                SetSaberVisible(_playerController.leftSaber, false);
                SetTrailWidth(0f);
                _saberClashChecker.enabled = false;
                _saberBurnMarkArea.enabled = false;
                _saberBurnMarkSparkles.enabled = false;
            }

            if (Config.hideSaberEffects)
            {
                SetTrailWidth(0f);
                _saberClashChecker.enabled = false;
                _saberBurnMarkArea.enabled = false;
                _saberBurnMarkSparkles.enabled = false;
            }

            if (Config.boxing)
            {
                SetTrailWidth(0.05f);
                _rightSaberTransform.localScale = new Vector3(4, 4, 0.25f);
                _leftSaberTransform.localScale = new Vector3(4, 4, 0.25f);
            }

            if (Config.headbang)
            {
                SetTrailWidth(0f);
                _rightSaberTransform.localScale = new Vector3(2, 2, 0.5f);
                _leftSaberTransform.localScale = new Vector3(2, 2, 0.5f);
            }

            if (Config.vacuum)
            {
                if (_gameEnergyCounter == null)
                    _gameEnergyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
                _gameEnergyCounter.SetPrivateField("_hitBombEnergyDrain", 0f);
            }

            if (GameObject.Find("vacuum_saber_right"))
            {
                SetTrailWidth(0f);
            }

            if (Config.feet)
            {
                // removing bombs that overlap with notes
                // most of maps don't need this, so I might remove this codes
                
                // testing
                if (_beatmapObjectManager == null)
                    _beatmapObjectManager = Resources.FindObjectsOfTypeAll<BeatmapObjectManager>().FirstOrDefault();
                _beatmapObjectManager.noteWasSpawnedEvent += OnNoteWasSpawned;

                _prevNoteTime = 0;
                _prevBombTime = 0;
                _bombList.Clear();
                _noteList.Clear();
            }

            if (Config.feetAvatar || Config.feetTracker || Config.fourSabers || Config.ninjaMaster)
            {
                StartCoroutine(FindAvatarCoroutine());
            }

            UpdateSaberActive();

            _init = true;
        }

        private IEnumerator FindAvatarCoroutine()
        {
            // need some wait
            _avatarType = AvatarType.None;
            for (int i = 0; i < 3; i++)
            {
                Logger.log.Debug("VRM Search");
                GameObject vrm = GameObject.Find("VMCReceiver/VRM");
                if (vrm == null)
                {
                    Logger.log.Debug("VRM Search2");
                    vrm = GameObject.Find("VRM");
                }

                if (vrm != null)
                {
                    Logger.log.Debug("VRM Found");
                    _footL = vrm.GetComponent<Animator>()?.GetBoneTransform(HumanBodyBones.LeftFoot)?.transform;
                    _footR = vrm.GetComponent<Animator>()?.GetBoneTransform(HumanBodyBones.RightFoot)?.transform;
                    _hips = vrm.GetComponent<Animator>()?.GetBoneTransform(HumanBodyBones.Hips)?.transform;
                    _head = vrm.GetComponent<Animator>()?.GetBoneTransform(HumanBodyBones.Head)?.transform;
                    if (_footL != null && _footR != null)
                    {
                        _avatarType = AvatarType.VMCAvatar;
                    }
                    break;
                }
                else
                {
                    Logger.log.Debug("VRM NotFound");
                    Logger.log.Debug("CustomAvatar Search");
                    GameObject customAvatar = GameObject.Find("_CustomAvatar(Clone)");
                    if (customAvatar != null)
                    {
                        Logger.log.Debug("CustomAvatar Found");
                        _footL = customAvatar.GetComponentInChildren<Animator>()?.GetBoneTransform(HumanBodyBones.LeftFoot)?.transform;
                        _footR = customAvatar.GetComponentInChildren<Animator>()?.GetBoneTransform(HumanBodyBones.RightFoot)?.transform;
                        _hips = customAvatar.GetComponentInChildren<Animator>()?.GetBoneTransform(HumanBodyBones.Hips)?.transform;
                        _head = customAvatar.GetComponentInChildren<Animator>()?.GetBoneTransform(HumanBodyBones.Head)?.transform;
                        if (_footL != null && _footR != null)
                        {
                            _avatarType = AvatarType.CustomAvatar;
                        }
                        break;
                    }
                    else
                    {
                        Logger.log.Debug("CustomAvatar NotFound");
                    }
                }

                yield return new WaitForSecondsRealtime(0.1f);
            }

            // found feet
            if (_footL != null && _footR != null)
            {
                Logger.log.Debug($"FootL: {_footL.position.x}, {_footL.position.y}, {_footL.position.z}");
                Logger.log.Debug($"FootR: {_footR.position.x}, {_footR.position.y}, {_footR.position.z}");

                if (Config.ninjaMaster)
                {
                    if (!Config.ninjaMasterHideHand)
                    {
                        _saberL2 = Instantiate(_playerController.leftSaber);
                        _saberR2 = Instantiate(_playerController.rightSaber);
                    }

                    if (!Config.ninjaMasterHideFoot)
                    {
                        _saberFootL = Instantiate(_playerController.leftSaber);
                        _saberFootR = Instantiate(_playerController.rightSaber);
                        _saberFootL.transform.localScale = new Vector3(2, 2, 0.25f);
                        _saberFootR.transform.localScale = new Vector3(2, 2, 0.25f);

                        _saberFootL2 = Instantiate(_playerController.leftSaber);
                        _saberFootR2 = Instantiate(_playerController.rightSaber);
                        _saberFootL2.transform.localScale = new Vector3(2, 2, 0.25f);
                        _saberFootR2.transform.localScale = new Vector3(2, 2, 0.25f);
                    }

                    if (!Config.ninjaMasterHideWaist)
                    {
                        _saberWaistL = Instantiate(_playerController.leftSaber);
                        _saberWaistR = Instantiate(_playerController.rightSaber);
                    }

                    if (!Config.ninjaMasterHideMouth)
                    {
                        _saberMouthL = Instantiate(_playerController.leftSaber);
                        _saberMouthR = Instantiate(_playerController.rightSaber);
                    }

                    if (!Config.ninjaMasterHideHead)
                    {
                        _saberHeadL = Instantiate(_playerController.leftSaber);
                        _saberHeadR = Instantiate(_playerController.rightSaber);
                        _saberHeadL.transform.localScale = new Vector3(1, 1, 0.25f);
                        _saberHeadR.transform.localScale = new Vector3(1, 1, 0.25f);
                    }
                }
                else if (Config.fourSabers)
                {
                    _saberFootL = Instantiate(_playerController.leftSaber);
                    _saberFootR = Instantiate(_playerController.rightSaber);
                    _saberFootL.transform.localScale = new Vector3(2, 2, 0.25f);
                    _saberFootR.transform.localScale = new Vector3(2, 2, 0.25f);
                }
                else
                {
                    /*
                    // customshoes check
                    bool isCustomShoes = false;
                    foreach (Transform transform in _playerController.leftSaber.GetComponentsInChildren<Transform>())
                    {
                        string name = transform.name.ToLower();
                        if ((name.IndexOf("feetsaber") >= 0) || name.IndexOf("shoes") >= 0)
                        {
                            Logger.log.Debug("CustomShoes Found.");
                            isCustomShoes = true;
                            break;
                        }
                    }

                    if (!isCustomShoes)
                    */
                    {
                        SetTrailWidth(0f);
                    }
                    _playerController.rightSaber.transform.localScale = new Vector3(2, 2, 0.25f);
                    _playerController.leftSaber.transform.localScale = new Vector3(2, 2, 0.25f);
                }

                // need some wait
                yield return new WaitForSecondsRealtime(0.1f);
                if (Config.hideSaberEffects)
                {
                    SetTrailWidth(0f);
                }
            }
            else
            {
                Logger.log.Debug($"Foot NotFound");

                if (Config.feetTracker)
                {
                    SetTrailWidth(0f);
                    _playerController.rightSaber.transform.localScale = new Vector3(2, 2, 0.25f);
                    _playerController.leftSaber.transform.localScale = new Vector3(2, 2, 0.25f);
                }

                // for debug
                //StartCoroutine(EnumAllObjectsCoroutine());
            }
        }

        private void OnNotGameCore()
        {
            // cleanup
            if (_feetSaber != null) Destroy(_feetSaber);
            if (_saberFootL != null) Destroy(_saberFootL);
            if (_saberFootR != null) Destroy(_saberFootR);
            if (_saberFootL2 != null) Destroy(_saberFootL2);
            if (_saberFootR2 != null) Destroy(_saberFootR2);
            if (_saberL2 != null) Destroy(_saberL2);
            if (_saberR2 != null) Destroy(_saberR2);
            if (_saberWaistL != null) Destroy(_saberWaistL);
            if (_saberWaistR != null) Destroy(_saberWaistR);
            if (_saberMouthL != null) Destroy(_saberMouthL);
            if (_saberMouthR != null) Destroy(_saberMouthR);
            if (_saberHeadL != null) Destroy(_saberHeadL);
            if (_saberHeadR != null) Destroy(_saberHeadR);
        }

        private float _prevNoteTime;
        private float _prevBombTime;
        private List<NoteController> _bombList = new List<NoteController>();
        private List<NoteController> _noteList = new List<NoteController>();

        private void OnNoteWasSpawned(NoteController noteController)
        {
            float time;
            if (noteController.noteData.noteType == NoteType.Bomb)
            {
                //Logger.log.Debug($"Spawn Bomb: {noteController.noteData.id} : {noteController.noteData.time}");

                time = noteController.noteData.time;
                if (_prevBombTime != 0 && time != _prevBombTime)
                {
                    _prevBombTime = 0;
                    _bombList.Clear();
                }

                bool isDissolved = false;
                if (time == _prevNoteTime)
                {
                    foreach (NoteController note in _noteList)
                    {
                        if (noteController.noteTransform.position.x == note.noteTransform.position.x)
                        {
                            //Logger.log.Debug($"Dissolve: {noteController.noteData.id} : {noteController.noteData.time}");
                            noteController.Dissolve(0);
                            isDissolved = true;
                            break;
                        }
                    }
                }

                if (!isDissolved)
                {
                    _prevBombTime = time;
                    _bombList.Add(noteController);
                }
            }
            else
            {
                //Logger.log.Debug($"Spawn Note: {noteController.noteData.id} : {noteController.noteData.time}");

                time = noteController.noteData.time;
                if (time != _prevNoteTime)
                {
                    _prevNoteTime = time;
                    _noteList.Clear();
                }
                _noteList.Add(noteController);

                if (time == _prevBombTime)
                {
                    for (int i = _bombList.Count - 1; i >= 0; i--)
                    {
                        if (noteController.noteTransform.position.x == _bombList[i].noteTransform.position.x)
                        {
                            //Logger.log.Debug($"Dissolve: {_bombList[i].noteData.id} : {_bombList[i].noteData.time}");
                            _bombList[i].Dissolve(0);
                            _bombList.RemoveAt(i);
                        }
                    }
                }
            }
        }

        private void Start()
        {
            Logger.log?.Debug($"{name}: Start()");
        }

        private Vector3 _prevHeadPos;
        private Vector3 _prevLeftHandlePos;
        private Vector3 _prevRightHandlePos;

        private void AdjustPosAndRot(Transform transform, float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
        {
            transform.Translate(posX * 0.01f, posY * 0.01f, posZ * 0.01f, Space.Self);
            //transform.Rotate(rotX, rotY, rotZ, Space.Self);
            transform.Rotate(rotX, 0, 0, Space.Self);
            transform.Rotate(0, rotY, 0, Space.Self);
            transform.Rotate(0, 0, rotZ, Space.Self);
        }

        private void Update()
        {
            if (!_init)
            {
                return;
            }
                
            if (Config.boxing)
            {
                _rightSaberTransform.Translate(0, 0, -0.23f, Space.Self);
                _leftSaberTransform.Translate(0, 0, -0.23f, Space.Self);
            }

            if (Config.headbang)
            {
                _rightSaberTransform.rotation = _playerController.headRot;
                _leftSaberTransform.rotation = _playerController.headRot;

                _rightSaberTransform.Rotate(new Vector3(270, 0, 0));
                _leftSaberTransform.Rotate(new Vector3(270, 0, 0));

                _rightSaberTransform.position = _playerController.headPos;
                _leftSaberTransform.position = _playerController.headPos;

                _rightSaberTransform.Translate(0.05f, 0, -0.2f, Space.Self);
                _leftSaberTransform.Translate(-0.05f, 0, -0.2f, Space.Self);
            }

            if (Config.reverseGrip)
            {
                _leftSaberTransform.transform.Rotate(0.0f, 180.0f, 0.0f);
                _leftSaberTransform.transform.Translate(0.0f, 0.0f, 0.18f, Space.Self);

                _rightSaberTransform.transform.Rotate(0.0f, 180.0f, 0.0f);
                _rightSaberTransform.transform.Translate(0.0f, 0.0f, 0.18f, Space.Self);
            }

            if (Config.feetTracker || ((Config.feetAvatar || Config.fourSabers || Config.ninjaMaster) && (_footR != null) && (_footL != null)))
            {
                Transform footSaberL = null;
                Transform footSaberR = null;

                if (!(Config.ninjaMaster && Config.ninjaMasterHideFoot))
                {
                    if (Config.fourSabers || Config.ninjaMaster)
                    {
                        footSaberL = _saberFootL.transform;
                        footSaberR = _saberFootR.transform;
                    }
                    else
                    {
                        footSaberL = _leftSaberTransform;
                        footSaberR = _rightSaberTransform;
                    }

                    Quaternion footRotationL;
                    Quaternion footRotationR;
                    Vector3 footPositionL;
                    Vector3 footPositionR;
                    if (Config.feetTracker)
                    {
                        leftFootTracker.TryGetFeatureValue(CommonUsages.devicePosition, out footPositionL);
                        rightFootTracker.TryGetFeatureValue(CommonUsages.devicePosition, out footPositionR);
                        leftFootTracker.TryGetFeatureValue(CommonUsages.deviceRotation, out footRotationL);
                        rightFootTracker.TryGetFeatureValue(CommonUsages.deviceRotation, out footRotationR);
                    }
                    else
                    {
                        footPositionL = _footL.position;
                        footPositionR = _footR.position;
                        footRotationL = _footL.rotation;
                        footRotationR = _footR.rotation;
                    }

                    footSaberL.position = footPositionL;
                    footSaberR.position = footPositionR;
                    footSaberL.rotation = footRotationL;
                    footSaberR.rotation = footRotationR;

                    if (Config.feetTracker)
                    {
                        AdjustPosAndRot(footSaberL,
                            -1 * Config.trackerFootPosX, Config.trackerFootPosY, Config.trackerFootPosZ,
                            Config.trackerFootRotX, -1 * Config.trackerFootRotY, -1 * Config.trackerFootRotZ);
                        AdjustPosAndRot(footSaberR,
                            Config.trackerFootPosX, Config.trackerFootPosY, Config.trackerFootPosZ,
                            Config.trackerFootRotX, Config.trackerFootRotY, Config.trackerFootRotZ);
                    }
                    else
                    {
                        if (_avatarType == AvatarType.VMCAvatar)
                        {
                            AdjustPosAndRot(footSaberL,
                                -1 * Config.vmcAvatarFootPosX, Config.vmcAvatarFootPosY, Config.vmcAvatarFootPosZ,
                                Config.vmcAvatarFootRotX, -1 * Config.vmcAvatarFootRotY, -1 * Config.vmcAvatarFootRotZ);
                            AdjustPosAndRot(footSaberR,
                                Config.vmcAvatarFootPosX, Config.vmcAvatarFootPosY, Config.vmcAvatarFootPosZ,
                                Config.vmcAvatarFootRotX, Config.vmcAvatarFootRotY, Config.vmcAvatarFootRotZ);
                        }
                        else if (_avatarType == AvatarType.CustomAvatar)
                        {
                            AdjustPosAndRot(footSaberL,
                                -1 * Config.customAvatarFootPosX, Config.customAvatarFootPosY, Config.customAvatarFootPosZ,
                                Config.customAvatarFootRotX, -1 * Config.customAvatarFootRotY, -1 * Config.customAvatarFootRotZ);
                            AdjustPosAndRot(footSaberR,
                                Config.customAvatarFootPosX, Config.customAvatarFootPosY, Config.customAvatarFootPosZ,
                                Config.customAvatarFootRotX, Config.customAvatarFootRotY, Config.customAvatarFootRotZ);
                        }
                    }

                    if (Config.fourSabers || Config.ninjaMaster)
                    {
                        _saberFootL.ManualUpdate();
                        _saberFootR.ManualUpdate();
                    }
                }

                if (Config.ninjaMaster)
                {
                    if (Config.ninjaMasterHideHand)
                    {
                        _playerController.leftSaber.gameObject.SetActive(false);
                        _playerController.rightSaber.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (Config.ninjaMasterHideHandR)
                        {
                            _playerController.rightSaber.gameObject.SetActive(false);
                            _saberL2.gameObject.SetActive(false);
                        }
                        else
                        {
                            _saberL2.transform.position = _rightSaberTransform.position;
                            _saberL2.transform.rotation = _rightSaberTransform.rotation;
                            _saberL2.transform.Rotate(0.0f, 180.0f, 0.0f);
                            _saberL2.transform.Translate(0.0f, 0.0f, Config.ninjaMasterSaberSeparation, Space.Self);
                            _saberL2.ManualUpdate();
                        }

                        if (Config.ninjaMasterHideHandL)
                        {
                            _playerController.leftSaber.gameObject.SetActive(false);
                            _saberR2.gameObject.SetActive(false);
                        }
                        else
                        {
                            _saberR2.transform.position = _leftSaberTransform.position;
                            _saberR2.transform.rotation = _leftSaberTransform.rotation;
                            _saberR2.transform.Rotate(0.0f, 180.0f, 0.0f);
                            _saberR2.transform.Translate(0.0f, 0.0f, Config.ninjaMasterSaberSeparation, Space.Self);
                            _saberR2.ManualUpdate();
                        }
                    }

                    if (!Config.ninjaMasterHideFoot)
                    {
                        _saberFootL2.transform.position = footSaberR.position;
                        _saberFootL2.transform.rotation = footSaberR.rotation;
                        _saberFootL2.transform.Rotate(0.0f, 180.0f, 0.0f);

                        _saberFootR2.transform.position = footSaberL.position;
                        _saberFootR2.transform.rotation = footSaberL.rotation;
                        _saberFootR2.transform.Rotate(0.0f, 180.0f, 0.0f);

                        _saberFootL2.ManualUpdate();
                        _saberFootR2.ManualUpdate();
                    }

                    if (!Config.ninjaMasterHideWaist)
                    {
                        _saberWaistL.transform.position = _hips.transform.position;
                        _saberWaistL.transform.rotation = _hips.transform.rotation;
                        _saberWaistL.transform.Rotate(0.0f, 270.0f, 0.0f);

                        _saberWaistR.transform.position = _hips.transform.position;
                        _saberWaistR.transform.rotation = _hips.transform.rotation;
                        _saberWaistR.transform.Rotate(0.0f, 90.0f, 0.0f);

                        _saberWaistL.ManualUpdate();
                        _saberWaistR.ManualUpdate();
                    }

                    Vector3 headPos = _head.transform.position;
                    Quaternion headRot = _head.transform.rotation;
                    if (!Config.ninjaMasterHideMouth)
                    {
                        _saberMouthL.transform.position = headPos;
                        _saberMouthL.transform.rotation = headRot;
                        _saberMouthL.transform.Rotate(0.0f, 270.0f, 0.0f);
                        _saberMouthL.transform.Translate(0.12f, 0.0f, 0.0f, Space.Self);

                        _saberMouthR.transform.position = headPos;
                        _saberMouthR.transform.rotation = headRot;
                        _saberMouthR.transform.Rotate(0.0f, 90.0f, 0.0f);
                        _saberMouthR.transform.Translate(-0.12f, 0.0f, 0.0f, Space.Self);

                        _saberMouthL.ManualUpdate();
                        _saberMouthR.ManualUpdate();
                    }

                    if (!Config.ninjaMasterHideHead)
                    {
                        _saberHeadL.transform.position = headPos;
                        _saberHeadL.transform.rotation = headRot;
                        _saberHeadL.transform.Rotate(0.0f, 270.0f, 0.0f);
                        _saberHeadL.transform.Rotate(315.0f, 0.0f, 0.0f);
                        _saberHeadL.transform.Translate(0.05f, 0.1f, 0.1f, Space.Self);

                        _saberHeadR.transform.position = headPos;
                        _saberHeadR.transform.rotation = headRot;
                        _saberHeadR.transform.Rotate(0.0f, 90.0f, 0.0f);
                        _saberHeadR.transform.Rotate(315.0f, 0.0f, 0.0f);
                        _saberHeadR.transform.Translate(-0.05f, 0.1f, 0.1f, Space.Self);

                        _saberHeadL.ManualUpdate();
                        _saberHeadR.ManualUpdate();
                    }
                }
            }

            if (Config.superhot)
            {
                float distance = 0;// = Vector3.Distance(_playerController.headPos, prevHeadPos);
                distance = Mathf.Max(distance, Vector3.Distance(_playerController.leftSaber.handlePos, _prevLeftHandlePos));
                distance = Mathf.Max(distance, Vector3.Distance(_playerController.rightSaber.handlePos, _prevRightHandlePos));
                distance = Mathf.Clamp01(distance * 50f);
                SetTimeScale(distance);

                _prevHeadPos = _playerController.headPos;
                _prevLeftHandlePos = _playerController.leftSaber.handlePos;
                _prevRightHandlePos = _playerController.rightSaber.handlePos;
            }
        }

        private void OnDestroy()
        {
            Logger.log?.Debug($"{name}: OnDestroy()");
            instance = null;
        }

        private void SetSaberVisible(Saber saber, bool active)
        {
            IEnumerable<MeshFilter> meshFilters = saber.transform.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter meshFilter in meshFilters)
            {
                meshFilter.gameObject.SetActive(active);

                MeshFilter filter = meshFilter.GetComponentInChildren<MeshFilter>();
                if (filter != null)
                {
                    filter.gameObject.SetActive(active);
                }
            }

            IEnumerable<Renderer> renders = saber.transform.GetComponentsInChildren<Renderer>();
            foreach (Renderer render in renders)
            {
                render.gameObject.SetActive(active);

                Renderer r = render.GetComponentInChildren<Renderer>();
                if (r != null)
                {
                    r.gameObject.SetActive(active);
                }
            }
        }

        private void SetTrailWidth(float trailWidth)
        {
            bool left = false;
            bool right = false;
            float newTrailWidth;
            foreach (XWeaponTrail trail in Resources.FindObjectsOfTypeAll<XWeaponTrail>())
            {
                //Logger.log.Debug($"trail: {trail.name}, {trail.transform?.parent?.name}, {trail.transform?.parent?.parent?.name}, {trail.transform?.parent?.parent?.parent?.name}, trailWidth={trail.GetPrivateField<float>("_trailWidth")}");
                if ((trail.name == "LeftSaber") || (trail.transform.parent?.name == "LeftSaber"))
                {
                    newTrailWidth = left ? 0f : trailWidth;
                    left = true;
                }
                else if ((trail.name == "RightSaber") || (trail.transform.parent?.name == "RightSaber"))
                {
                    newTrailWidth = right ? 0 : trailWidth;
                    right = true;
                }
                else
                {
                    newTrailWidth = 0;
                }
                trail.SetPrivateField("_trailWidth", newTrailWidth);
                //Logger.log.Debug($"trailWidth={trail.GetPrivateField<float>("_trailWidth")}");
            }
        }

        private void SetTrailWidth(GameObject parent, float trailWidth)
        {
            XWeaponTrail trail = parent.GetComponentInChildren<XWeaponTrail>();
            if (trail != null)
            { 
                trail.SetPrivateField("_trailWidth", trailWidth);
            }
        }

        private IEnumerator SetTimeScaleCoroutine(float timeScale)
        {
            float amount = 0;
            float currentTimeScale = _audioTimeSyncController.timeScale;
            while (amount < 1)
            {
                SetTimeScale(Mathf.Lerp(currentTimeScale, timeScale, amount));
                amount += 0.1f;
                yield return new WaitForFixedUpdate();
            }
        }

        private void SetTimeScale(float timeScale)
        {
            _audioTimeSyncController.SetPrivateField("_timeScale", timeScale);
            _audioSource.pitch = timeScale;
        }
    }
}
