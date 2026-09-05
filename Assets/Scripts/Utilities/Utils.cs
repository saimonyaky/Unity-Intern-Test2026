using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Utils
{
    public static NormalItem.eNormalType GetRandomNormalType()
    {
        Array values = Enum.GetValues(typeof(NormalItem.eNormalType));
        NormalItem.eNormalType result = (NormalItem.eNormalType)values.GetValue(URandom.Range(0, values.Length));

        return result;
    }

    public static NormalItem.eNormalType GetRandomNormalTypeExcept(NormalItem.eNormalType[] types)
    {
        List<NormalItem.eNormalType> list = Enum.GetValues(typeof(NormalItem.eNormalType)).Cast<NormalItem.eNormalType>().Except(types).ToList();

        int rnd = URandom.Range(0, list.Count);
        NormalItem.eNormalType result = list[rnd];

        return result;
    }

    public static List<NormalItem.eNormalType> CreateRandomTypes(int boardSizeX, int boardSizeY)
    {
        int cellCount = boardSizeX * (boardSizeY - 1);

        NormalItem.eNormalType[] allTypes = Enum.GetValues(typeof(NormalItem.eNormalType)).Cast<NormalItem.eNormalType>().ToArray();

        List<NormalItem.eNormalType> result = new List<NormalItem.eNormalType>();

        foreach (NormalItem.eNormalType type in allTypes)
        {
            result.Add(type);
            result.Add(type);
            result.Add(type);
        }

        while (result.Count < cellCount)
        {
            int randomIndex = URandom.Range(0, allTypes.Length - 1);

            NormalItem.eNormalType randomType = allTypes[randomIndex];

            result.Add(randomType);
            result.Add(randomType);
            result.Add(randomType);
        }

        for (int i = result.Count - 1; i > 0; i--)
        {
            int j = URandom.Range(0, i);

            var temp = result[i];
            result[i] = result[j];
            result[j] = temp;
        }
        return result;
    }
}
