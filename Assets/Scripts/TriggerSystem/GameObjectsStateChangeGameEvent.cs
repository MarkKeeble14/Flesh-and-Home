using UnityEngine;

public class GameObjectsStateChangeGameEvent : GameEvent
{
    [SerializeField] private SerializableDictionary<GameObject, StateChange> gameObjectStateChangeDict = new SerializableDictionary<GameObject, StateChange>();

    protected override void Activate()
    {
        foreach (SerializableKeyValuePair<GameObject, StateChange> obj in gameObjectStateChangeDict.ToList())
        {
            obj.Key.SetActive(obj.Value == StateChange.ENABLE ? true : false);
        }
    }
}
