using BS_Utils.Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xft;

namespace NalulunaModifier
{
    public class NalulunaModifierController : MonoBehaviour
    {
        public static NalulunaModifierController instance { get; private set; }

        private AudioTimeSyncController _audioTimeSyncController;
        private AudioSource _audioSource;
        private PlayerController _playerController;
        private Transform _rightSaberTransform;
        private Transform _leftSaberTransform;
        private SaberClashChecker _saberClashChecker;
        private SaberBurnMarkArea _saberBurnMarkArea;
        private SaberBurnMarkSparkles _saberBurnMarkSparkles;
        private BeatmapObjectSpawnController _beatmapObjectSpawnController;
        private GameEnergyCounter _gameEnergyCounter;
        private GamePause _gamePause;

        private bool _init;
        private float _originalTimeScale;

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

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            _init = false;

            if (newScene.name == "GameCore")
            {
                Config.Read();
                if (Config.parabola || Config.noBlue || Config.noRed || Config.redToBlue || Config.blueToRed || Config.centering || Config.superhot || Config.vacuum)
                {
                    ScoreSubmission.ProlongedDisableSubmission(Plugin.Name);
                }
                else
                {
                    ScoreSubmission.RemoveProlongedDisable(Plugin.Name);
                }

                StartCoroutine(OnGameCore());
            }
        }

        public void OnPauseResume()
        {
            UpdateSaberActive();
        }

        private void UpdateSaberActive()
        {
            _playerController.leftSaber.gameObject.SetActive(Config.blueToRed || !(Config.noRed || Config.redToBlue));
            _playerController.rightSaber.gameObject.SetActive(Config.redToBlue || !(Config.noBlue || Config.blueToRed));

            if (Config.vacuum)
            {
                _playerController.leftSaber.gameObject.SetActive(false);
            }
        }

        private IEnumerator OnGameCore()
        {
            if (_audioTimeSyncController == null)
            {
                _audioTimeSyncController = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
                _audioSource = _audioTimeSyncController.GetPrivateField<AudioSource>("_audioSource");
            }
            _originalTimeScale = _audioTimeSyncController.timeScale;

            if (_gamePause == null)
                _gamePause = Resources.FindObjectsOfTypeAll<GamePause>().FirstOrDefault();
            _gamePause.didResumeEvent += OnPauseResume;

            // wait for custom saber
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
            }

            if (Config.headbang)
            {
                SetTrailWidth(0f);
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

            UpdateSaberActive();

            _init = true;
        }

        private void Start()
        {
            Logger.log?.Debug($"{name}: Start()");
        }

        private Vector3 _prevHeadPos;
        private Vector3 _prevLeftHandlePos;
        private Vector3 _prevRightHandlePos;

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

                _rightSaberTransform.localScale = new Vector3(4, 4, 0.25f);
                _leftSaberTransform.localScale = new Vector3(4, 4, 0.25f);
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

                _rightSaberTransform.localScale = new Vector3(2, 2, 0.5f);
                _leftSaberTransform.localScale = new Vector3(2, 2, 0.5f);
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
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;

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
                filter?.gameObject.SetActive(active);
            }
        }

        private void SetTrailWidth(float trailWidth)
        {
            IEnumerable<XWeaponTrail> trails = Resources.FindObjectsOfTypeAll<XWeaponTrail>();
            foreach (XWeaponTrail trail in trails)
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
