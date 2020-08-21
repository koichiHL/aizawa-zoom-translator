using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TranslateTarget : int
{
    ja_JP,
    en,
}

public class TranslatePacker
{
    Dictionary<string, string> packed = new Dictionary<string, string>();
    
    public void Add(TranslateTarget t, string v)
    {
        packed.Add(t.ToString("G"), v);
    }

    public Dictionary<string, string> GetPack()
    {
        return packed;
    }

}
