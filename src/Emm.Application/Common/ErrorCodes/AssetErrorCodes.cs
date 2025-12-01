namespace Emm.Application.Common.ErrorCodes;

/// <summary>
/// Asset error codes
/// </summary>
public static class AssetErrorCodes
{
    public const string NotFound = "ASSET_NOT_FOUND";
    public const string AlreadyExists = "ASSET_ALREADY_EXISTS";
    public const string CodeExists = "ASSET_CODE_EXISTS";
    public const string InUse = "ASSET_IN_USE";
    public const string CannotDelete = "ASSET_CANNOT_DELETE";
}

/// <summary>
/// Asset Type error codes
/// </summary>
public static class AssetTypeErrorCodes
{
    public const string NotFound = "ASSET_TYPE_NOT_FOUND";
    public const string InUse = "ASSET_TYPE_IN_USE";
    public const string AlreadyExists = "ASSET_TYPE_ALREADY_EXISTS";
    public const string CannotDelete = "ASSET_TYPE_CANNOT_DELETE";
}

/// <summary>
/// Asset Model error codes
/// </summary>
public static class AssetModelErrorCodes
{
    public const string NotFound = "ASSET_MODEL_NOT_FOUND";
    public const string InUse = "ASSET_MODEL_IN_USE";
    public const string AlreadyExists = "ASSET_MODEL_ALREADY_EXISTS";
    public const string CannotDelete = "ASSET_MODEL_CANNOT_DELETE";
}
