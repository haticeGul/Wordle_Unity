
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[]
    {KeyCode.A, KeyCode.B, KeyCode.C ,KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W,KeyCode.X, KeyCode.Y, KeyCode.Z };


    private Row[] rows;

    private string[] validwords;
    public int rowIndex;
    public int columnIndex;
    public string word;

    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

    [Header("UI")]
    public Button newWordButton;
    public Button tryAgainButton;

  

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();

    }

    private void Start()
    {
        LoadData();
        NewGame();
    }

    public void NewGame()
    {
        ClearBoard();
        SetRandomWord();
        enabled = true;
    }

    public void TryAgain()
    {
        ClearBoard();
        enabled = true;
    }
    private void ClearBoard()
    {
        for(int row =0; row< rows.Length; row++)
        {
            for(int col =0; col <rows[row].tiles.Length; col++)
            {
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetState(emptyState);
            }
        }

        rowIndex = 0;
        columnIndex = 0;
    }


    private void LoadData()
    {
        //burada db baglantisi kuralým
        TextAsset textFile = Resources.Load("words") as TextAsset;
        validwords = textFile.text.Split('\n');
    }

    private void SetRandomWord()
    {
        word = validwords[Random.Range(0, validwords.Length)];
       
    }

    private void Update()
    {
        Row currentRow = rows[rowIndex];

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);
        }
        else if (columnIndex >= currentRow.tiles.Length)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(currentRow);
            }
        }
        else
        {
            for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
            {
                if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                    rows[rowIndex].tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                    currentRow.tiles[columnIndex].SetState(occupiedState);
                    columnIndex++;
                    break;
                }
            }

        }

    }

    private void SubmitRow(Row row)
    {
        string remaining = word;
        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.letter == word[i])
            {
                tile.SetState(correctState);
                //bulduðu karakteri replace ettim.Ýndeks ile etmek önemli ayný harften olabilir
                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
                Debug.Log(remaining);
            }
            else if (!word.Contains(tile.letter))
            {
                tile.SetState(incorrectState);
            }
        }

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.state != correctState && tile.state != incorrectState)
            {
                if (remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);

                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else
                {
                    tile.SetState(incorrectState);
                }

            }
        }

        if (HasWon(row))
        {
            enabled = false;
        }

        //bir sonraki satýr
        rowIndex++;
        columnIndex = 0;

        if(rowIndex >= rows.Length)
        {
            enabled = false;
        }
    }

    private bool HasWon(Row row)
    {
        foreach (var tile in row.tiles)
        {
            if(tile.state != correctState)
            {
                return false;
            }
        }
        
        return true;
    }

    private void OnEnable()
    {
        tryAgainButton.gameObject.SetActive(false);
        newWordButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        tryAgainButton.gameObject.SetActive(true);
        newWordButton.gameObject.SetActive(true);
    }
}
