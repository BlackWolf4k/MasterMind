using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class main : MonoBehaviour
{
    // Grandezze piano di gioco
    private static int BOARD_ROWS = 11;
    private static int BOARD_COLUMNS = 5;
    // Grandezze contenitore pin
    private static int CONTAINER_ROWS = 4;
    private static int CONTAINER_COLUMNS = 2;

    private Pin_t[,] board = new Pin_t[ BOARD_ROWS, BOARD_COLUMNS ];
    private Pin_t[,] container = new Pin_t[ CONTAINER_ROWS, CONTAINER_COLUMNS ];
    private Pin_t[,] small = new Pin_t[ BOARD_ROWS, BOARD_COLUMNS ];

    private int[,] smallPinCoordinates = new int[ BOARD_COLUMNS, 2 ];

    private GameObject[] objects_array = new GameObject[8];

    [Header( "Prefabs Big Pins" ) ]

    [SerializeField] GameObject White_p;
    [SerializeField] GameObject Black_p;
    [SerializeField] GameObject Red_p;
    [SerializeField] GameObject Yellow_p;

    [SerializeField] GameObject Orange_p;
    [SerializeField] GameObject Purple_p;
    [SerializeField] GameObject Green_p;
    [SerializeField] GameObject Blue_p;

    [Header( "Prefabs Small Pins" ) ]
    [SerializeField] GameObject white_p;
    [SerializeField] GameObject red_p;

    [Header( "GUI" ) ]
    [SerializeField] TextMeshProUGUI GUI;

    [Header( "Board Pieces" ) ]
    [SerializeField] GameObject board_piece;
    [SerializeField] GameObject final_board_piece;

    private Vector2 mousePosition;

    private int actual_level = 0;

    Vector3 offset = new Vector3( 5, 0, 5 );

    private Pin_t pinToMove;

    void Start()
    {
        for ( int k = 0; k < BOARD_ROWS; k++ )
        {
            GameObject.Instantiate( board_piece ).GetComponent<Transform>().transform.position = new Vector3( k * 10, 0, 0 );
            if ( k == BOARD_ROWS - 1 )
                GameObject.Instantiate( final_board_piece ).GetComponent<Transform>().transform.position = new Vector3( k * 10 + 10, 0, 0 );
        }

        int i = 3, j = 1;
        spawnContainerPiece( White_p, i, j, "white" );
        i--;
        spawnContainerPiece( Red_p, i , j, "red" );
        i--;
        spawnContainerPiece( Orange_p, i , j, "orange" );
        i--;
        spawnContainerPiece( Green_p, i , j, "green" );

        j--;
        i = 3;
        spawnContainerPiece( Black_p, i , j, "black" );
        i--;
        spawnContainerPiece( Yellow_p, i , j, "yellow" );
        i--;
        spawnContainerPiece( Purple_p, i , j, "purple" );
        i--;
        spawnContainerPiece( Blue_p, i , j, "blue" );

        makeCoderPins();

        // possibili coordinante dei pin piccoli
        smallPinCoordinates[ 0, 0 ] = 3; smallPinCoordinates[ 0, 1 ] = 0;
        smallPinCoordinates[ 1, 0 ] = 3; smallPinCoordinates[ 1, 1 ] = 4;
        smallPinCoordinates[ 2, 0 ] = 7; smallPinCoordinates[ 2, 1 ] = 0;
        smallPinCoordinates[ 3, 0 ] = 7; smallPinCoordinates[ 3, 1 ] = 4;
    }

    void Update()
    {
        mousePositionUpdate();
        int x = (int)mousePosition.x;
        int y = (int)mousePosition.y;

        if ( actual_level == BOARD_ROWS )
        {
            GUI.text = "HAI PERSO!";
            Application.Quit();
        }

        if ( Input.GetMouseButtonDown( 0 ) ) // mouse pulsante sinistro seleziona pin dal contenitore
            selectContainerPin( x, y );

        if ( Input.GetMouseButtonDown( 1 ) && x == actual_level ) // mouse pulsante destro piazza pin
            if ( pinToMove )
                move_copy( pinToMove.gameObject, x, y );

        if ( Input.GetMouseButtonDown( 2 ) && x == actual_level ) // mouse pultanse centrale rimuove pin
            removePiece( x, y );
    }

    private void makeCoderPins() // in modo "random" genera dei pezzi per il coder
    {
        int x, y;
        Vector3 offset_ = new Vector3( 12.5f, 0, 10.3f );
        for ( int i = 0; i < BOARD_COLUMNS - 1; i++ )
        {
            x = Random.Range( 0, 4 ); y = Random.Range( 0, 2 );
            board[ 10, i ] = container[ x, y ];
            GameObject obj = GameObject.Instantiate( board[ 10, i ].gameObject );
            obj.name = obj.name.Substring( 0, obj.name.Length - 7); // lungheza -7 per rimuovere il (Clone)
            move( obj.GetComponent<Pin_t>(), BOARD_ROWS, i, offset_ );
        }
    }

    private void spawnContainerPiece( GameObject prefab, int x, int y, string name ) // genera i pin nel contenitore
    {
        GameObject obj = GameObject.Instantiate( prefab );
        obj.name = name;
        Pin_t pin = obj.GetComponent<Pin_t>(); // crea il pin, dato il prefabbricato
        container[x, y] = pin;
        move( pin, x, y + 5, offset ); // muove il pezzo in posizione ( + 5 per posizionare nel contenitore )
    }

    private void spawnSmallPin( GameObject prefab, int x, int y, string name )
    {
        int i = ( y > 0 ) ? 1 : 0;
        GameObject obj = GameObject.Instantiate( prefab );
        obj.name = name;
        Pin_t pin = obj.GetComponent<Pin_t>();
        small[actual_level, i ] = pin;
        Vector3 offset_ = new Vector3( ( actual_level * 10 + x ), 0, ( y + 3 ) );
        move( pin, 0, 0, offset_ );
    }

    private void move_copy( GameObject obj, int x, int y ) // come move ma invece che muovere l'oggetto ne crea una copia
    {
        GameObject new_obj = GameObject.Instantiate( obj );
        new_obj.name = new_obj.name.Substring( 0, new_obj.name.Length - 7); // lungheza -7 per rimuovere il (Clone)
        Pin_t pin = new_obj.GetComponent<Pin_t>();
        if ( x < BOARD_ROWS - 1 && y > 0 && y < BOARD_COLUMNS )
        {
            if ( board[x, y] != null )
                Destroy( board[ x, y ].gameObject );
            board[x, y] = pin;
            move( pin, x, y, offset );
        }
    }

    private void move( Pin_t Piece, int x, int y, Vector3 offset_ ) // muove un pezzo
    {
        Piece.GetComponent<Transform>().transform.position = new Vector3( x * 10, 0, y * 10 ) + offset_;
    }

    private void removePiece( int x, int y ) // rimuove un pezzo
    {
        if ( x < BOARD_ROWS - 1 && y < BOARD_COLUMNS && board[ x, y ] != null )
            Destroy( board[x, y].gameObject );
    }

    private void selectContainerPin( int x, int y ) // seleziona il pin nel contenitore
    {
        if ( x < CONTAINER_ROWS && y - 5 >= 0 && y - 5 < CONTAINER_COLUMNS )
            pinToMove = container[ x, y - 5 ];
        if ( x == 4 && y == 5 )
            check();
    }

    private void mousePositionUpdate() // Posizione del mouse
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if ( Physics.Raycast(ray, out hit ) && hit.transform.gameObject != null )
            if ( hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 10 || hit.transform.gameObject.layer == 11 )
            {
                mousePosition.x = (int)( hit.transform.gameObject.transform.position.x / 10f );
                mousePosition.y = (int)( hit.transform.gameObject.transform.position.z / 10f );
            }
    }

    private void check() // Controlla i pin
    {
        int all_filled = 0;
        for ( int i = 0; i < BOARD_COLUMNS; i++ ) // Controlla tutti i pin della fila attuale
            if ( board[actual_level, i] != null )
                all_filled++;

        // Se tutti i pin sono pieni si prosegue altrimenti da messaggio di errore
        if ( all_filled == BOARD_COLUMNS - 1 )
        {
            int correct = 0;
            int[,] results = new int[ BOARD_COLUMNS - 1, BOARD_COLUMNS - 1 ];
            for ( int i = 1; i < BOARD_COLUMNS; i++ )
            {
                for ( int j = 0; j < BOARD_COLUMNS - 1; j++ )
                {
                    if ( board[actual_level, i].name == board[BOARD_ROWS - 1, j ].name ) // mette nella matrice il risultato del controllo per ogni pin
                    {
                        if ( i == j + 1 )
                        {
                            results[ i - 1, j ] = 2;
                            correct++;
                        }
                        else
                            results[ i - 1, j ] = 1;
                    }
                }
            }
            for ( int i = 0; i < BOARD_COLUMNS - 1; i++ ) // inserisce i pin rossi o bianchi
            {
                bool red = false;
                for ( int j = 0; j < BOARD_COLUMNS - 1; j++ )
                    if ( results[i, j] == 2 )
                    {
                        spawnSmallPin( red_p, smallPinCoordinates[i, 0], smallPinCoordinates[i, 1], "red_small" );
                        red = true;
                    }
                for ( int j = 0; j < BOARD_COLUMNS - 1 && red == false; j++ )
                    if ( results[i, j] == 1 )
                        spawnSmallPin( white_p, smallPinCoordinates[i, 0], smallPinCoordinates[i, 1], "white_small" );
            }

            /*for ( int i = 0; i < BOARD_COLUMNS - 1; i++ )
                Debug.Log( results[ i, 0] + " " + results[ i, 1 ] + " " + results[ i, 2 ] + " " + results[ i, 3 ] );*/

            if ( correct == BOARD_COLUMNS - 1 )
            {
                GUI.text = "HAI VINTO!";
                Application.Quit();
            }
            actual_level++;
        }
        else
            StartCoroutine( not_filled() );
    }

    IEnumerator not_filled() // messaggio di 1 secondo che non tutti i pin sono pieni
    {
        GUI.text = "Non Piene";
        yield return new WaitForSeconds( 1 );
        GUI.text = "";
    }
}