using UnityEngine;

public class StartMenuManager : MonoBehaviour
{
    [Header("场景")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO teachScene;          // 教学关卡场景

    [Header("出生点")]
    public Vector3 teachScenePosition = Vector3.zero;

    // 开始游戏按钮调用
    public void OnStartGame()
    {
        loadEventSO.RaiseLoadRequestEvent(teachScene, teachScenePosition, true);
    }

    // 退出游戏按钮调用
    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}