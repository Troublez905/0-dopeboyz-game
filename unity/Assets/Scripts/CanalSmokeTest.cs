using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Dopeboyz
{
    public sealed class CanalSmokeTest : MonoBehaviour
    {
        int failures;
        void OnEnable() { Application.logMessageReceived += OnLog; }
        void OnDisable() { Application.logMessageReceived -= OnLog; }
        void OnLog(string message,string stack,LogType type)
        {
            if(type==LogType.Error || type==LogType.Exception || type==LogType.Assert) failures++;
        }
        void Check(bool condition, string label)
        {
            Debug.Log("CANAL TEST " + (condition ? "PASS " : "FAIL ") + label);
            if (!condition) failures++;
        }
        IEnumerator Start()
        {
            yield return new WaitForSecondsRealtime(2);
            Check(GameManager.Instance != null && GameManager.Instance.currentState == GameState.MainMenu, "menu startup");
            Check(Time.timeScale == 0, "menu freezes simulation");
            string output = Path.Combine(Application.persistentDataPath,"CanalQA"); Directory.CreateDirectory(output);
            Screen.SetResolution(1280,720,false);
            yield return new WaitForSecondsRealtime(1);
            ScreenCapture.CaptureScreenshot(Path.Combine(output,"menu-1280.png"));
            yield return new WaitForSecondsRealtime(.5f);
            GameSession.Instance.Begin(false);
            float deadline = Time.realtimeSinceStartup + 90;
            while (GameSession.Instance.IsLoading && Time.realtimeSinceStartup < deadline) yield return null;
            Check(!GameSession.Instance.IsLoading, "async scene load completes");
            yield return new WaitForSecondsRealtime(2);
            Check(GameManager.Instance != null && PlayerController.Instance != null, "gameplay references");
            if (PlayerController.Instance == null) { Application.Quit(1); yield break; }
            Check(FindObjectsByType<GameSession>(FindObjectsSortMode.None).Length == 1, "single session");
            Check(FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length == 1, "single gameplay manager");
            Check(FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length == 1, "single audio listener");
            var walls = FindObjectsByType<WallTag>(FindObjectsSortMode.None);
            Check(walls.Length >= 3,"claimable walls");
            Check(GameObject.Find("October Bench") != null,"October bench instantiated");
            Check(GameObject.Find("October YardFence") != null,"October fence instantiated");
            Check(GameObject.Find("October WallLight") != null,"October light instantiated");
            var companion = FindFirstObjectByType<OctoberCompanion>();
            Check(companion != null && companion.animator != null && companion.animator.runtimeAnimatorController != null,"October animated companion");
            var session = GameSession.Instance; var campaign = CampaignManager.Instance;
            session.Award("qa-one",150); int credits = campaign.levelCredits;
            session.Award("qa-one",150);
            Check(session.Level == 2 && campaign.levelCredits == credits,"XP duplicate prevention");
            Check(campaign.BuyUpgrade("speed"),"purchase upgrade");
            float speed = PlayerController.Instance.moveSpeed; campaign.RefreshStats();
            Check(Mathf.Approximately(speed,PlayerController.Instance.moveSpeed),"stat calculation idempotent");
            Check(session.Save(),"save succeeds");
            yield return new WaitForSecondsRealtime(.5f);
            ScreenCapture.CaptureScreenshot(Path.Combine(output,"gameplay-1280.png"));
            yield return new WaitForSecondsRealtime(1);
            Screen.SetResolution(1920,1080,false);
            yield return new WaitForSecondsRealtime(1);
            ScreenCapture.CaptureScreenshot(Path.Combine(output,"gameplay-1920.png"));
            session.Begin(true);
            deadline = Time.realtimeSinceStartup + 90;
            while(session.IsLoading && Time.realtimeSinceStartup < deadline) yield return null;
            yield return new WaitForSecondsRealtime(1);
            Check(!session.IsLoading && session.Level == 2,"continue restores XP");
            Check(Mathf.Approximately(speed,PlayerController.Instance.moveSpeed),"continue restores upgrade");
            Check(FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length == 1,"reload has no duplicate managers");
            File.WriteAllText(Path.Combine(output,"smoke-result.txt"),"Failures: " + failures + "\nAutomated startup/save tests only; manual input and sound listening still required.");
            Debug.Log("CANAL TEST COMPLETE failures=" + failures);
            yield return new WaitForSecondsRealtime(2);
            Application.Quit(failures == 0 ? 0 : 1);
        }
    }
}
