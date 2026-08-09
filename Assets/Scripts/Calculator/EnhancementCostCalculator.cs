using UnityEngine;

public static class EnhancementCostCalculator
{
    private const int MIN_LEVEL = 1;
    private const int MAX_LEVEL = 100;
    private const int BOUNDARY_LEVEL = 97;

    private const long SMOOTH_START_COST = 2000;
    private const long SMOOTH_END_COST = 10000000;
    private const int SMOOTH_STEPS = 96;

    private const long JUMP_MULTIPLIER = 10;

    public static long GetUpgradeCost(int currentLevel)
    {
        if (currentLevel < MIN_LEVEL || currentLevel >= MAX_LEVEL)
        {
            return 0;
        }

        if (currentLevel <= BOUNDARY_LEVEL)
        {
            return CalculateSmoothCost(currentLevel);
        }

        return CalculateJumpCost(currentLevel);
    }

    private static long CalculateSmoothCost(int level)
    {
        double t = (double)(level - 1) / SMOOTH_STEPS;
        double logCost = Mathf.Log(SMOOTH_START_COST) + t * (Mathf.Log(SMOOTH_END_COST) - Mathf.Log(SMOOTH_START_COST));
        return (long)System.Math.Round(System.Math.Exp(logCost));
    }

    private static long CalculateJumpCost(int level)
    {
        int jumpStep = level - BOUNDARY_LEVEL;
        long cost = SMOOTH_END_COST;
        for (int i = 0; i < jumpStep; i++)
        {
            cost *= JUMP_MULTIPLIER;
        }
        return cost;
    }
}