using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GeneratorController : MonoBehaviour
{
    [SerializeField] private GameObject block;
    [SerializeField] private int playerFacingDirection;
    [SerializeField] private CardinalDirection cardinalDirection;

    private float vertical;
    private float horizontal;

    [Range(5, 15)] public int renderDistance;

    private void Start()
    {
        for (int x = -renderDistance; x < renderDistance; x++)
        {
            for (int z = -renderDistance; z < renderDistance; z++)
            {
                block.transform.position = new Vector3(this.transform.position.x + x * 2, 0, this.transform.position.x + z * 2);
                Instantiate(block);
            }
        }
    }
    private void Update()
    {
        playerFacingDirection = (int)this.transform.eulerAngles.y;
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(horizontal, 0f, vertical);

        CheckCardinalDirection();

        if (dir.magnitude >= 0.1f) BlockPassingController(); //вызывается только если начинаеться движение
    }

    private void UpdateLandscape()
    {
        switch (cardinalDirection)
        {
            case CardinalDirection.North:
                break;

            case CardinalDirection.South:
                break;

            case CardinalDirection.East:
                break;

            case CardinalDirection.West:
                break;

            case CardinalDirection.NorthEast:
                break;

            case CardinalDirection.NorthWest:
                break;

            case CardinalDirection.SouthEast:
                break;

            case CardinalDirection.SouthWest:
                break;
        }
    }

    private void BlockPassingController() //функция для замера количества прйденных блоков
    {

    }

    private void CheckCardinalDirection()
    {
        if (playerFacingDirection > 30 && playerFacingDirection < 80) cardinalDirection = CardinalDirection.NorthEast;

        else if (playerFacingDirection > 100 && playerFacingDirection < 170) cardinalDirection = CardinalDirection.SouthEast;

        else if (playerFacingDirection > 190 && playerFacingDirection < 260) cardinalDirection = CardinalDirection.SouthWest;

        else if (playerFacingDirection > 280 && playerFacingDirection < 330) cardinalDirection = CardinalDirection.NorthWest;

        else if ((playerFacingDirection > 330 && playerFacingDirection < 360) || (playerFacingDirection < 30 && playerFacingDirection > 0))
            cardinalDirection = CardinalDirection.North;

        else if (playerFacingDirection > 80 && playerFacingDirection < 100) cardinalDirection = CardinalDirection.East;

        else if (playerFacingDirection > 170 && playerFacingDirection < 190) cardinalDirection = CardinalDirection.South;

        else if (playerFacingDirection > 260 && playerFacingDirection < 280) cardinalDirection = CardinalDirection.West;
    }
    public enum CardinalDirection
    {
        North,
        South,
        East,
        West,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
    }
}