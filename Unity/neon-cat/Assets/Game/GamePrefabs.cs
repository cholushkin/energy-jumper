using System.Collections;
using System.Collections.Generic;
using GameLib.Alg;
using ResourcesHelper;
using UnityEngine;

public class GamePrefabs : Singleton<GamePrefabs>
{
    public GroupHolder<GameObject> GameEntities;
    public GroupHolder<Material> Materials;
}
