using UnityEngine;


public struct OnPlayerAte    { public int eatenFishSize; public int newPlayerSize; }
public struct OnPlayerDied   { public string reason; }  
public struct OnHookCaught   { public Transform hookTransform; }
public struct OnScoreChanged { public int score; }
public struct OnGameStarted  { }
public struct OnGameOver     { public string reason; public int finalScore; }
