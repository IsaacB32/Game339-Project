using UnityEngine;

[CreateAssetMenu(fileName = "NewOrder", menuName = "Cursed Coffee/Customer Order")]
public class CustomerOrder : ScriptableObject
{
    public string orderText;
    public Sprite drinkIcon;
    public Vector2 drinkIconSize = new Vector2(100, 100);
    public Sprite customerSprite;
    public Vector2 customerSize = new Vector2(400, 700);
}