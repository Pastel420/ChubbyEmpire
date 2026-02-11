using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Game Scence/GameSceneSO")]

public class GameSceneSO : ScriptableObject 
{
    public SceneType sceneType;
    public AssetReference sceneReference;
}