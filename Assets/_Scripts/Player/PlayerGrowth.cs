using System.Collections.Generic;
using UnityEngine;

public class PlayerGrowth : MonoBehaviour
{
    [Header("Scale/Growth Settings")]
    [SerializeField] private List<PlayerFoodGrowthData> stages;
    [Header("RunTime")]
    [SerializeField] private GrowthStage currentStage = GrowthStage.Small;

    private int eaten1, eaten5, eaten10;
    

    private PlayerFoodGrowthData Current => stages.Find(s => s.stage == currentStage);


    public bool CanEatFood(FoodData food)
    {
        return food.points <= Current.maxAllowedLimit ;
    }

    public void RegisterFood(FoodData food)
    {
        switch(food.points)
        {
            case 1: eaten1++; break;
            case 5: eaten5++; break;
            case 10: eaten10++; break;
        }

        //CheckGrowth();
    }

    private void CheckGrowth()
    {
        var data = Current;
        if(eaten1 >= data.smallFish && eaten5 >= data.mediumFish && eaten10 >= data.largeFish)
        {
            AdvanceStage();
        }
    }

    private void AdvanceStage()
    {
        if(currentStage == GrowthStage.Kraken)
        {
            return;
        }
        currentStage++ ;

        //StartCoroutine
    }

    // private IEnumerator ScaleUp(Vector3 target)
    // {
        
    // }


}