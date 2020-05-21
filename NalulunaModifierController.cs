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

        private PlayerController _playerController;
        private Transform _rightSaberTransform;
        private Transform _leftSaberTransform;
        private SaberClashChecker _saberClashChecker;
        private SaberBurnMarkArea _saberBurnMarkArea;
        private SaberBurnMarkSparkles _saberBurnMarkSparkles;
        private BeatmapObjectSpawnController _beatmapObjectSpawnController;

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
            if (newScene.name == "GameCore")
            {
                Config.Read();
                if (Config.parabola || Config.noBlue || Config.noRed || Config.redToBlue || Config.blueToRed || Config.centering)
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

        private IEnumerator OnGameCore()
        {
            if (Config.centering)
            {
                if (_beatmapObjectSpawnController == null)
                    _beatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault();
                BeatmapObjectSpawnMovementData beatmapObjectSpawnMovementData = _beatmapObjectSpawnController.GetPrivateField<BeatmapObjectSpawnMovementData>("_beatmapObjectSpawnMovementData");
                Vector3 leftBase = beatmapObjectSpawnMovementData.GetNoteOffset(0, NoteLineLayer.Base);
                Vector3 rightTop = beatmapObjectSpawnMovementData.GetNoteOffset(3, NoteLineLayer.Top);
                NoteJumpManualUpdate.center = (leftBase + rightTop) / 2;
                //Logger.log.Debug($"leftBase={leftBase}, rightTop={rightTop}");
            }

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

            _playerController.rightSaber.gameObject.SetActive(Config.blueToRed || !(Config.noRed || Config.redToBlue));
            _playerController.leftSaber.gameObject.SetActive(Config.redToBlue || !(Config.noBlue || Config.blueToRed));

            if (Config.hideSabers)
            {
                SetSaberVisible(_playerController.rightSaber, false);
                SetSaberVisible(_playerController.leftSaber, false);
                SetTrailWidth(0f);
                _saberClashChecker.enabled = false;
                _saberBurnMarkArea.enabled = false;
                _saberBurnMarkSparkles.enabled = false;
            }

            if (Config.boxing)
            {
                SetTrailWidth(0.05f);
            }
        }

        private void Start()
        {
            Logger.log?.Debug($"{name}: Start()");
        }

        private void Update()
        {
            if (Config.boxing)
            {
                _rightSaberTransform.Translate(0, 0, -0.23f, Space.Self);
                _leftSaberTransform.Translate(0, 0, -0.23f, Space.Self);

                _rightSaberTransform.localScale = new Vector3(4, 4, 0.25f);
                _leftSaberTransform.localScale = new Vector3(4, 4, 0.25f);
            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;

            Logger.log?.Debug($"{name}: OnDestroy()");
            instance = null;
        }
    }
}
