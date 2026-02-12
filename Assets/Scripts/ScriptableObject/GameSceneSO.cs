using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Game Scene/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    // 枚举定义在类内部
    public enum SceneType
    {
        Start,      // 开始界面
        Teach,      // 教学关卡
        Level,      // 普通关卡（P, S）
        Boss,       // Boss关卡（C）
        End         // 结束界面
    }

    public SceneType sceneType;
    public AssetReference sceneReference;
}