﻿using UnityEngine;

public class FirstSceneControllerBase : IUserInterface, IQueryStatus, ISetStatus, IScore {
    private static FirstSceneControllerBase _gameSceneController;
    private GameStatus gameStatus;
    private SceneStatus sceneStatus;
    //public PhysicActionManager scene;
    //public CCActionManager scene;
    public IActionController scene;
    private UFOFactoryBase ufoFactory = UFOFactoryBase.GetFactory();

    public static FirstSceneControllerBase GetFirstSceneControllerBase() {
        return _gameSceneController ?? (_gameSceneController = new FirstSceneControllerBase());
    }

    public void SendUFO() {
        int UFOCount = scene.GetUFONum();
        var UFOList = ufoFactory.PrepareUFO(UFOCount);
        scene.SendUFO(UFOList);
    }

    public void DestroyUFO(GameObject UFO) {
        scene.DestroyUFO(UFO);
        ufoFactory.RecycleUFO(UFO);
    }

    public GameStatus QueryGameStatus() {
        return gameStatus;
    }
    public SceneStatus QuerySceneStatus() {
        return sceneStatus;
    }

    public void SetGameStatus(GameStatus _gameStatus) {
        gameStatus = _gameStatus;
    }
    public void SetSceneStatus(SceneStatus _sceneStatus) {
        sceneStatus = _sceneStatus;
    }

    public void AddScore() {
        GameModel.GetGameModel().AddScore();
    }
    public void SubScore() {
        GameModel.GetGameModel().SubScore();
    }
    public int GetScore() {
        return GameModel.GetGameModel().Score;
    }

    public void Update() {
        scene.SceneUpdate();
    }
}

public class FirstSceneController : MonoBehaviour {
    // Use this for initialization
    void Start() {
        FirstSceneControllerBase.GetFirstSceneControllerBase().scene = gameObject.AddComponent<PhysicActionManager>();
        //FirstSceneControllerBase.GetFirstSceneControllerBase().scene = gameObject.AddComponent<CCActionManager>();
    }
}
