//for my mod video
//change ninjasaber mode to feetsaber 2-player. no meaning without 2-player mod
//#define ninjasaber_for_2p

using BS_Utils.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace NalulunaModifier
{
    public class NalulunaModifierController : MonoBehaviour
    {
        enum AvatarType { None, VMCAvatar, CustomAvatar }

        public static NalulunaModifierController instance { get; private set; }

        private AudioTimeSyncController _audioTimeSyncController;
        private AudioSource _audioSource;
        private PauseController _pauseController;

        private SaberManager _saberManager;
        private NoteCutter _noteCutter;
        private PlayerTransforms _playerTransform;
        private Transform _rightSaberTransform;
        private Transform _leftSaberTransform;

        private SaberClashChecker _saberClashChecker;
        private SaberBurnMarkArea _saberBurnMarkArea;
        private SaberBurnMarkSparkles _saberBurnMarkSparkles;

        private GameEnergyCounter _gameEnergyCounter;

        private BeatmapObjectManager _beatmapObjectManager;
        private BeatmapObjectSpawnController _beatmapObjectSpawnController;

        private ColorManager _colorManager;

        private Transform _footL;
        private Transform _footR;
        private AvatarType _avatarType;
        private GameObject _feetSaber;

        private Saber _saberFootL;
        private Saber _saberFootR;
        private Saber _saberFootL2;
        private Saber _saberFootR2;
        private Saber _saberLeft2;
        private Saber _saberRight2;
        private Saber _saberWaistL;
        private Saber _saberWaistR;
        private Saber _saberMouthL;
        private Saber _saberMouthR;
        private Saber _saberHeadL;
        private Saber _saberHeadR;
        private Transform _hips;
        private Transform _head;
#if ninjasaber_for_2p
        private Transform _footL2;
        private Transform _footR2;
#endif

        private InputDevice leftFootTracker = new InputDevice();
        private InputDevice rightFootTracker = new InputDevice();

        private bool _init;

        internal static readonly string saberFootLName = "saberFootL";
        internal static readonly string saberFootRName = "saberFootR";
        internal static readonly string saberLeft2Name = "saberLeft2";
        internal static readonly string saberRight2Name = "saberRight2";

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
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        public void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            Logger.log.Debug($"OnActiveSceneChanged: {newScene.name}");

            _init = false;

            if (newScene.name == "GameCore")
            {
                Logger.log.Debug($"Mode: {BS_Utils.Plugin.LevelData.Mode}");

                if (BS_Utils.Plugin.LevelData.Mode != BS_Utils.Gameplay.Mode.Standard)
                {
                    Config.feetModifiers = false;
                    Config.feet = false;
                    Config.flatNotes = false;
                    Config.noDirection = false;
                    Config.ignoreBadColor = false;
                    Config.feetAvatar = false;
                    Config.feetTracker = false;

                    Config.ninjaModifiers = false;
                    Config.fourSabers = false;
                    Config.reverseGrip = false;
                    Config.topNotesToFeet = false;
                    Config.middleNotesToFeet = false;
                    Config.bottomNotesToFeet = false;

                    Config.parabola = false;
                    Config.noBlue = false;
                    Config.noRed = false;
                    Config.redToBlue = false;
                    Config.blueToRed = false;

                    Config.boxing = false;
                    Config.hideSabers = false;
                    Config.hideSaberEffects = false;
                    Config.centering = false;

                    Config.headbang = false;
                    Config.superhot = false;
                    Config.vacuum = false;
                    Config.ninjaMaster = false;
                    Config.beatWalker = false;

                    ModifierUI.instance.updateUI();

                    return;
                }

                Config.Read();

                if (Config.parabola || Config.noBlue || Config.noRed || Config.redToBlue || Config.blueToRed || Config.centering ||
                    Config.feet || Config.noDirection || Config.flatNotes ||
                    Config.fourSabers || Config.topNotesToFeet || Config.middleNotesToFeet || Config.bottomNotesToFeet ||
                    Config.superhot || Config.vacuum || Config.ninjaMaster || Config.beatWalker)
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

                if (Config.feetTracker || Config.beatWalker)
                {
                    FindTrackers();
                }

                StartCoroutine(OnGameCoreCoroutine());
            }
            else if (newScene.name == "MainMenu")
            {
                OnMainMenu();
            }
        }

        public void OnPause()
        {
            if (_saberFootL != null) _saberFootL.gameObject.SetActive(false);
            if (_saberFootR != null) _saberFootR.gameObject.SetActive(false);
            if (_saberFootL2 != null) _saberFootL2.gameObject.SetActive(false);
            if (_saberFootR2 != null) _saberFootR2.gameObject.SetActive(false);
            if (_saberLeft2 != null) _saberLeft2.gameObject.SetActive(false);
            if (_saberRight2 != null) _saberRight2.gameObject.SetActive(false);
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
            if (_saberLeft2 != null) _saberLeft2.gameObject.SetActive(true);
            if (_saberRight2 != null) _saberRight2.gameObject.SetActive(true);
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
                _saberManager.leftSaber.gameObject.SetActive(false);
            }

            if (!(Config.redToBlue || !(Config.noBlue || Config.blueToRed)))
            {
                _saberManager.rightSaber.gameObject.SetActive(false);
            }

            if (Config.vacuum && !Config.ninjaMaster)
            {
                _saberManager.leftSaber.gameObject.SetActive(false);
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

            if (_pauseController == null)
            {
                _pauseController = Resources.FindObjectsOfTypeAll<PauseController>().FirstOrDefault();
                _pauseController.didPauseEvent += OnPause;
                _pauseController.didResumeEvent += OnPauseResume;
            }

            // wait for CustomSaber mod
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());
            yield return new WaitForSecondsRealtime(0.1f);

            if (_saberManager == null)
                _saberManager = Resources.FindObjectsOfTypeAll<SaberManager>().FirstOrDefault();
            if (_noteCutter == null)
            {
                CuttingManager cuttingManager = Resources.FindObjectsOfTypeAll<CuttingManager>().FirstOrDefault();
                _noteCutter = cuttingManager.GetPrivateField<NoteCutter>("_noteCutter");
            }
            if (_playerTransform == null)
                _playerTransform = Resources.FindObjectsOfTypeAll<PlayerTransforms>().FirstOrDefault();
            if (_rightSaberTransform == null)
                _rightSaberTransform = _saberManager.rightSaber.transform;
            if (_leftSaberTransform == null)
                _leftSaberTransform = _saberManager.leftSaber.transform;
            if (_saberClashChecker == null)
            {
                SaberClashEffect saberClashEffect = Resources.FindObjectsOfTypeAll<SaberClashEffect>().FirstOrDefault();
                if (saberClashEffect != null)
                {
                    _saberClashChecker = saberClashEffect.GetPrivateField<SaberClashChecker>("_saberClashChecker");
                }
            }
            if (_saberBurnMarkArea == null)
                _saberBurnMarkArea = Resources.FindObjectsOfTypeAll<SaberBurnMarkArea>().FirstOrDefault();
            if (_saberBurnMarkSparkles == null)
                _saberBurnMarkSparkles = Resources.FindObjectsOfTypeAll<SaberBurnMarkSparkles>().FirstOrDefault();

            if (Config.hideSabers)
            {
                SetSaberVisible(_saberManager.rightSaber, false);
                SetSaberVisible(_saberManager.leftSaber, false);
                SetTrailWidth(0f);
                HarmonyPatches.SaberClashCheckerAreSabersClashing.disabled = true;
                _saberBurnMarkArea.enabled = false;
                _saberBurnMarkSparkles.enabled = false;
            }

            if (Config.hideSaberEffects)
            {
                SetTrailWidth(0f);
                HarmonyPatches.SaberClashCheckerAreSabersClashing.disabled = true;
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

                if (GameObject.Find("vacuum_saber_right"))
                {
                    SetTrailWidth(0f);
                }
            }

            if (Config.feet)
            {
                // removing bombs that overlap with notes
                // most of maps don't need this, so I might remove this codes
                
                // testing
                if (_beatmapObjectManager == null)
                    _beatmapObjectManager = _pauseController.GetPrivateField<BeatmapObjectManager>("_beatmapObjectManager");
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

        /*
        private IEnumerator GetSpawnCenterCoroutine()
        {
            // need some wait
            yield return new WaitForSecondsRealtime(1f);

            if (_beatmapObjectSpawnController == null)
                _beatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault();
            BeatmapObjectSpawnMovementData beatmapObjectSpawnMovementData = _beatmapObjectSpawnController.GetPrivateField<BeatmapObjectSpawnMovementData>("_beatmapObjectSpawnMovementData");
            Vector3 leftBase = beatmapObjectSpawnMovementData.GetNoteOffset(0, NoteLineLayer.Base);
            Vector3 rightTop = beatmapObjectSpawnMovementData.GetNoteOffset(3, NoteLineLayer.Top);
            center = (leftBase + rightTop) / 2;
            Logger.log.Debug($"leftBase={leftBase.x}, {leftBase.y}, {leftBase.z}");
            Logger.log.Debug($"rightTop={rightTop.x}, {rightTop.y}, {rightTop.z}");
        }
        */

        private IEnumerator FindAvatarCoroutine()
        {
            // need some wait
            _avatarType = AvatarType.None;
            for (int i = 0; i < 3; i++)
            {
                //Logger.log.Debug("VRM Search");
                GameObject vrm = GameObject.Find("VMCReceiver/VRM");
                if (vrm == null)
                {
                    //Logger.log.Debug("VRM Search2");
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

#if ninjasaber_for_2p
                    vrm = GameObject.Find("VMCReceiver2/VRM");
                    _footL2 = vrm.GetComponent<Animator>()?.GetBoneTransform(HumanBodyBones.LeftFoot)?.transform;
                    _footR2 = vrm.GetComponent<Animator>()?.GetBoneTransform(HumanBodyBones.RightFoot)?.transform;
#endif
                    break;
                }
                else
                {
                    //Logger.log.Debug("VRM NotFound");
                    //Logger.log.Debug("CustomAvatar Search");
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
                        Logger.log.Debug("Avatar NotFound");
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
                        _saberLeft2 = CopySaber(_saberManager.leftSaber);
                        _saberRight2 = CopySaber(_saberManager.rightSaber);

                        _saberLeft2.name = saberLeft2Name;
                        _saberRight2.name = saberRight2Name;
                    }

                    if (!Config.ninjaMasterHideFoot)
                    {
                        _saberFootL = CopySaber(_saberManager.leftSaber);
                        _saberFootR = CopySaber(_saberManager.rightSaber);
                        _saberFootL.transform.localScale = new Vector3(2, 2, 0.25f);
                        _saberFootR.transform.localScale = new Vector3(2, 2, 0.25f);

                        _saberFootL2 = CopySaber(_saberManager.leftSaber);
                        _saberFootR2 = CopySaber(_saberManager.rightSaber);
                        _saberFootL2.transform.localScale = new Vector3(2, 2, 0.25f);
                        _saberFootR2.transform.localScale = new Vector3(2, 2, 0.25f);
                    }

                    if (!Config.ninjaMasterHideWaist)
                    {
                        _saberWaistL = CopySaber(_saberManager.leftSaber);
                        _saberWaistR = CopySaber(_saberManager.rightSaber);
                    }

                    if (!Config.ninjaMasterHideMouth)
                    {
                        _saberMouthL = CopySaber(_saberManager.leftSaber);
                        _saberMouthR = CopySaber(_saberManager.rightSaber);
                    }

                    if (!Config.ninjaMasterHideHead)
                    {
                        _saberHeadL = CopySaber(_saberManager.leftSaber);
                        _saberHeadR = CopySaber(_saberManager.rightSaber);
                        _saberHeadL.transform.localScale = new Vector3(1, 1, 0.25f);
                        _saberHeadR.transform.localScale = new Vector3(1, 1, 0.25f);
                    }
                }
                else if (Config.fourSabers)
                {
                    _saberFootL = CopySaber(_saberManager.leftSaber);
                    _saberFootR = CopySaber(_saberManager.rightSaber);
                    _saberFootL.transform.localScale = new Vector3(2, 2, 0.25f);
                    _saberFootR.transform.localScale = new Vector3(2, 2, 0.25f);

                    _saberFootL.name = saberFootLName;
                    _saberFootR.name = saberFootRName;

#if ninjasaber_for_2p
                    _rightSaberTransform.localScale = new Vector3(2, 2, 0.25f);
                    _leftSaberTransform.localScale = new Vector3(2, 2, 0.25f);
#endif
                }
                else
                {
                    /*
                    // customshoes check
                    bool isCustomShoes = false;
                    foreach (Transform transform in _saberManager.leftSaber.GetComponentsInChildren<Transform>())
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
                    _saberManager.rightSaber.transform.localScale = new Vector3(2, 2, 0.25f);
                    _saberManager.leftSaber.transform.localScale = new Vector3(2, 2, 0.25f);
                }

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
                    _saberManager.rightSaber.transform.localScale = new Vector3(2, 2, 0.25f);
                    _saberManager.leftSaber.transform.localScale = new Vector3(2, 2, 0.25f);
                }

                // for debug
                //StartCoroutine(EnumAllObjectsCoroutine());
            }
        }

        private Saber CopySaber(Saber saber)
        {
            if (_colorManager == null)
            {
                _colorManager = Resources.FindObjectsOfTypeAll<ColorManager>().FirstOrDefault();
            }

            Saber result = Instantiate(saber);
            result.transform.SetParent(saber.transform.parent, false);
            result.transform.localPosition = Vector3.zero;
            result.transform.localRotation = Quaternion.identity;

            // for check components
            /*
            foreach (var monoBehaviour in result.GetComponents<MonoBehaviour>())
            {
                Logger.log.Debug($"{result.name} has {monoBehaviour.GetType().Name}");
            }
            */

            SaberModelContainer saberModelContainer = result.GetComponent<SaberModelContainer>();
            DestroyImmediate(saberModelContainer);

            VRController vrController = result.GetComponent<VRController>();
            DestroyImmediate(vrController);

            // color
            foreach (SetSaberGlowColor setSaberGlowColor in result.GetComponentsInChildren<SetSaberGlowColor>())
            {
                setSaberGlowColor.SetPrivateField("_colorManager", _colorManager);
                setSaberGlowColor.saberType = saber.saberType;
                setSaberGlowColor.Start();
            }

            // trail
            // it might need some wait for customsabers?
            Color color = _colorManager.ColorForSaberType(saber.saberType);
            SaberModelController saberModelController = saber.GetComponentInChildren<SaberModelController>();
            SaberModelController.InitData initData = saberModelController.GetPrivateField<SaberModelController.InitData>("_initData");
            Color trailTintColor = initData.trailTintColor;
            foreach (SaberTrail saberTrail in result.GetComponentsInChildren<SaberTrail>())
            {
                saberTrail.Setup((color * trailTintColor).linear, saber.movementData);
            }

            return result;
        }

        private void OnMainMenu()
        {
            // cleanup
            if (_feetSaber != null) Destroy(_feetSaber);
            if (_saberFootL != null) Destroy(_saberFootL);
            if (_saberFootR != null) Destroy(_saberFootR);
            if (_saberFootL2 != null) Destroy(_saberFootL2);
            if (_saberFootR2 != null) Destroy(_saberFootR2);
            if (_saberLeft2 != null) Destroy(_saberLeft2);
            if (_saberRight2 != null) Destroy(_saberRight2);
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
            if (noteController.noteData.colorType == ColorType.None)
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

        private float _prevDistance;
        private Vector3 _prevHeadPos;
        private Vector3 _prevLeftHandlePos;
        private Vector3 _prevRightHandlePos;

        private void AdjustPosAndRot(Transform transform, float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
        {
            transform.Translate(posX * 0.01f, posY * 0.01f, posZ * 0.01f, Space.Self);
            transform.Rotate(rotX, 0, 0, Space.Self);
            transform.Rotate(0, rotY, 0, Space.Self);
            transform.Rotate(0, 0, rotZ, Space.Self);
        }

        private void UpdateAdditionalSaber(Saber saber)
        {
            saber.ManualUpdate();
            if (_noteCutter != null)
            {
                _noteCutter.Cut(saber);
            }
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
                _rightSaberTransform.rotation = _playerTransform.headRot;
                _leftSaberTransform.rotation = _playerTransform.headRot;

                _rightSaberTransform.Rotate(new Vector3(270, 0, 0));
                _leftSaberTransform.Rotate(new Vector3(270, 0, 0));

                _rightSaberTransform.position = _playerTransform.headPos;
                _leftSaberTransform.position = _playerTransform.headPos;

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

#if ninjasaber_for_2p
                        _leftSaberTransform.position = _footL2.position;
                        _leftSaberTransform.rotation = _footL2.rotation;
                        _rightSaberTransform.position = _footR2.position;
                        _rightSaberTransform.rotation = _footR2.rotation;
#endif
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
                        UpdateAdditionalSaber(_saberFootL);
                        UpdateAdditionalSaber(_saberFootR);
                    }
                }

                if (Config.ninjaMaster)
                {
                    if (Config.ninjaMasterHideHand)
                    {
                        _saberManager.leftSaber.gameObject.SetActive(false);
                        _saberManager.rightSaber.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (Config.ninjaMasterHideHandR)
                        {
                            _saberManager.rightSaber.gameObject.SetActive(false);
                            _saberLeft2.gameObject.SetActive(false);
                        }
                        else
                        {
                            _saberLeft2.transform.position = _rightSaberTransform.position;
                            _saberLeft2.transform.rotation = _rightSaberTransform.rotation;
                            _saberLeft2.transform.Rotate(0.0f, 180.0f, 0.0f);
                            _saberLeft2.transform.Translate(0.0f, 0.0f, Config.ninjaMasterSaberSeparation, Space.Self);
                            UpdateAdditionalSaber(_saberLeft2);
                        }

                        if (Config.ninjaMasterHideHandL)
                        {
                            _saberManager.leftSaber.gameObject.SetActive(false);
                            _saberRight2.gameObject.SetActive(false);
                        }
                        else
                        {
                            _saberRight2.transform.position = _leftSaberTransform.position;
                            _saberRight2.transform.rotation = _leftSaberTransform.rotation;
                            _saberRight2.transform.Rotate(0.0f, 180.0f, 0.0f);
                            _saberRight2.transform.Translate(0.0f, 0.0f, Config.ninjaMasterSaberSeparation, Space.Self);
                            UpdateAdditionalSaber(_saberRight2);
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

                        UpdateAdditionalSaber(_saberFootL2);
                        UpdateAdditionalSaber(_saberFootR2);
                    }

                    if (!Config.ninjaMasterHideWaist)
                    {
                        _saberWaistL.transform.position = _hips.transform.position;
                        _saberWaistL.transform.rotation = _hips.transform.rotation;
                        _saberWaistL.transform.Rotate(0.0f, 270.0f, 0.0f);

                        _saberWaistR.transform.position = _hips.transform.position;
                        _saberWaistR.transform.rotation = _hips.transform.rotation;
                        _saberWaistR.transform.Rotate(0.0f, 90.0f, 0.0f);

                        UpdateAdditionalSaber(_saberWaistL);
                        UpdateAdditionalSaber(_saberWaistR);
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

                        UpdateAdditionalSaber(_saberMouthL);
                        UpdateAdditionalSaber(_saberMouthR);
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

                        UpdateAdditionalSaber(_saberHeadL);
                        UpdateAdditionalSaber(_saberHeadR);
                    }
                }
            }

            if (Config.superhot)
            {
                float distance = 0;// = Vector3.Distance(_playerController.headPos, prevHeadPos);
                distance = Mathf.Max(distance, Vector3.Distance(_saberManager.leftSaber.handlePos, _prevLeftHandlePos));
                distance = Mathf.Max(distance, Vector3.Distance(_saberManager.rightSaber.handlePos, _prevRightHandlePos));
                distance = Mathf.Clamp01(distance * 50f);
                SetTimeScale(distance);

                _prevHeadPos = _playerTransform.headPos;
                _prevLeftHandlePos = _saberManager.leftSaber.handlePos;
                _prevRightHandlePos = _saberManager.rightSaber.handlePos;
            }
            else if (Config.beatWalker)
            {
                leftFootTracker.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 footPositionL);
                rightFootTracker.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 footPositionR);

                float currentDistance = Mathf.Max(Vector3.Distance(footPositionL, _prevLeftHandlePos), Vector3.Distance(footPositionR, _prevRightHandlePos));
                //float distance = Mathf.Lerp(_prevDistance, currentDistance, Time.deltaTime);
                SetTimeScale(Mathf.Clamp01(currentDistance * 50f));
                _prevDistance = currentDistance;
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

        private Coroutine _setTrailWidthCoroutine;

        private IEnumerator SetTrailWidthCoroutine(float trailWidth)
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<SaberTrailRenderer>().Any());
            
            // need some wait. 0.02 is not enough for my environment.
            yield return new WaitForSecondsRealtime(0.03f);
            foreach (SaberTrailRenderer trail in Resources.FindObjectsOfTypeAll<SaberTrailRenderer>())
            {
                trail.SetPrivateField("_trailWidth", trailWidth);
            }

            // just in case
            yield return new WaitForSecondsRealtime(0.1f);
            foreach (SaberTrailRenderer trail in Resources.FindObjectsOfTypeAll<SaberTrailRenderer>())
            {
                trail.SetPrivateField("_trailWidth", trailWidth);
            }
        }

        private void SetTrailWidth(float trailWidth)
        {
            if (_setTrailWidthCoroutine != null)
            {
                StopCoroutine(_setTrailWidthCoroutine);
            }
            _setTrailWidthCoroutine = StartCoroutine(SetTrailWidthCoroutine(trailWidth));
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
