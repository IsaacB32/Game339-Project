using UnityEngine;

[CreateAssetMenu(fileName = "NewOrder", menuName = "Cursed Coffee/Customer Order")]
public class CustomerOrder : ScriptableObject
{
    [Header("Order")]
    public string orderText;
    public Sprite drinkIcon;
    public Vector2 drinkIconSize = new Vector2(100, 100);

    [Header("Customer")]
    public Sprite customerSprite;
    public Vector2 customerSize = new Vector2(400, 700);
    public float customerYPosition = 0f;
    public string customerBlurp;
}