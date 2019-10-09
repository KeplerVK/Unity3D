# UFO飞碟游戏
**[github项目传送门](https://github.com/KeplerVK/Unity3d/edit/master/week4/README.md)**

### 规则

1. 按下空格发射飞碟
1. 点击飞碟得分
1. 飞碟落地则扣分
1. 难度随次数增加

### 实现

#### 1. 代码文件目录

+ Controller
    + Interface.cs  
        *Interfaces and Enum variables*
    + FirstSceneController.cs  
        *FirstController logic codes*
    + Scene.cs  
        *Scene and Score Controller*
    + UFOFactory.cs  
        *UFO controller for creating and recycling UFO*
    + UserAction.cs  
        *User click or space press controller*
+ Model
    + GameModel.cs  
        *Score model*
    + UFOModel.cs  
        *UFO model*
+ View
    + UI.cs  
        *Game hint, score and status presentation*

#### 2. 具体实现

游戏主体大致分为：飞碟，用户和分数。分别对三个主体继续细致划分，以达到架构目的。
+ UFO
    + Model (*Speed, Color, Position, Direction*)
        ```cs
        public class UFOModel {
            public Color UFOColor;
            public Vector3 startPos;
            public Vector3 startDirection;
            public float UFOSpeed;

            public void Reset(int round) {
                UFOSpeed = 0.1f;
                if (round % 2 == 1) {
                    UFOColor = Color.red;
                    startPos = new Vector3(-5f, 3f, -15f);
                    startDirection = new Vector3(3f, 8f, 5f);
                } else {
                    UFOColor = Color.green;
                    startPos = new Vector3(5f, 3f, -15f);
                    startDirection = new Vector3(-3f, 8f, 5f);
                }
                for (int i = 1; i < round; i++) {
                    UFOSpeed *= 1.1f;
                }
            }
        }
        ```
    + Controller (*(=UFOFactory) Create, Recycle*)
        ```cs
        public class UFOFactoryBase : System.Object {
            public GameObject UFOPrefab;

            private static UFOFactoryBase ufoFactory;
            List<GameObject> inUseUFO;
            List<GameObject> notUseUFO;

            private UFOFactoryBase() {
                notUseUFO = new List<GameObject>();
                inUseUFO = new List<GameObject>();
            }

            public static UFOFactoryBase GetFactory() {
                return ufoFactory ?? (ufoFactory = new UFOFactoryBase());
            }

            public List<GameObject> PrepareUFO(int UFOnum) {
                for (int i = 0; i < UFOnum; i++) {
                    if (notUseUFO.Count == 0) {
                        GameObject disk = Object.Instantiate(UFOPrefab);
                        inUseUFO.Add(disk);
                    } else {
                        GameObject disk = notUseUFO[0];
                        notUseUFO.RemoveAt(0);
                        inUseUFO.Add(disk);
                    }
                }
                return inUseUFO;
            }

            public void RecycleUFO(GameObject UFO) {
                int index = inUseUFO.FindIndex(x => x == UFO);
                notUseUFO.Add(UFO);
                inUseUFO.RemoveAt(index);
            }
        }
        ```
+ 用户
    + ActionController (*Click, Press space button*)
        ```cs
        public class UserAction : MonoBehaviour {
            public GameObject planePrefab;

            GameStatus gameStatus;
            SceneStatus SceneStatus;

            IUserInterface uerInterface;
            IQueryStatus queryStatus;
            IScore changeScore;

            // Use this for initialization
            void Start() {
                // Initialize
            }

            // Update is called once per frame
            void Update() {
                gameStatus = queryStatus.QueryGameStatus();
                SceneStatus = queryStatus.QuerySceneStatus();

                if (gameStatus == GameStatus.Play) {
                    if (SceneStatus == SceneStatus.Waiting && Input.GetKeyDown("space")) {
                        uerInterface.SendUFO();
                    }
                    if (SceneStatus == SceneStatus.Shooting && Input.GetMouseButtonDown(0)) {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "UFO") {
                            uerInterface.DestroyUFO(hit.collider.gameObject);
                            changeScore.AddScore();
                        }
                    }
                }
            }
        }
        ```
+ 分数
    + StatusController (*Add, Subtract*)
        ```cs
        public class GameModel : IScore {
            public int Score { get; private set; }
            public int Round { get; private set; }
            private static GameModel gameModel;

            private GameModel() { Round = 1; }

            public static GameModel GetGameModel() {
                return gameModel ?? (gameModel = new GameModel());
            }

            public void AddScore() {
                Score += 10;
                if (CheckUpdate()) {
                    Round++;
                    FirstSceneControllerBase.GetFirstSceneControllerBase().Update();
                }
            }

            public void SubScore() {
                Score -= 10;
                if (Score < 0) {
                    FirstSceneControllerBase.GetFirstSceneControllerBase().SetGameStatus(GameStatus.Lose);
                }
            }

            public bool CheckUpdate() { return Score >= Round * 20; }

            public int GetScore() { return GameModel.GetGameModel().Score; }
        }
        ```

对于三者的交互请求，通过 FirstSceneController 和 Scene 来辅助管理，即实现用户按钮送飞碟，用户点击是否加分，是否扣分。

