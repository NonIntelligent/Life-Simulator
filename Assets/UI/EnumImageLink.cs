using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SpritePair
{
    public AIstates obj;
    public Sprite sprite;
}

public class EnumImageLink : MonoBehaviour
{
    public SpritePair[] iconPairs;
    static Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        // Refactor inputs from editor to an array that's indexed by the AI states enum
        sprites = new Sprite[(int)AIstates.NUM_STATES];
        for (int i = 0; i < iconPairs.Length; i++) {
            sprites[(int)iconPairs[i].obj] = iconPairs[i].sprite;
        }
    }

    public static Sprite getSpriteFromState(AIstates state) {
        return sprites[(int)state];
    }

}
