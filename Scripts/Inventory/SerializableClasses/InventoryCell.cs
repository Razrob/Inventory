public class InventoryCell
{
    public Item Item;
    public string CellID;
    public int ItemNumber;

    public InventoryCell()
    {
        Item = null;
        CellID = string.Empty;
        ItemNumber = 0;
    }
    public InventoryCell(Item _item)
    {
        Item = _item;
        CellID = _item.ItemID;
        ItemNumber = 1;
    }
    public InventoryCell(Item _item, int _itemNumber)
    {
        Item = _item;
        CellID = _item.ItemID;
        ItemNumber = _itemNumber;
    }
}
