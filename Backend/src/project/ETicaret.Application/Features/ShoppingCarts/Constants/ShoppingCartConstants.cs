namespace ETicaret.Application.Features.ShoppingCarts.Constants;

public static class ShoppingCartConstants
{
    #region Cache Groups
    public const string ShoppingCartsCacheGroup = "shopping-carts";
    public const string CartItemsCacheGroup = "cart-items";
    #endregion

    #region Business Rules
    public const int MAX_ITEMS_PER_CART = 50;
    public const int MAX_QUANTITY_PER_ITEM = 10;
    public const int MIN_QUANTITY_PER_ITEM = 1;
    #endregion

    #region Error Codes
    public static class ErrorCodes
    {
        public const string CartNotFound = "CART_NOT_FOUND";
        public const string CartItemNotFound = "CART_ITEM_NOT_FOUND";
        public const string CartNotOwnedByUser = "CART_NOT_OWNED_BY_USER";
        public const string MaxItemsExceeded = "MAX_ITEMS_EXCEEDED";
        public const string InvalidQuantity = "INVALID_QUANTITY";
        public const string ProductNotAvailable = "PRODUCT_NOT_AVAILABLE";
        public const string InsufficientStock = "INSUFFICIENT_STOCK";
        public const string DuplicateCartItem = "DUPLICATE_CART_ITEM";
    }
    #endregion
}