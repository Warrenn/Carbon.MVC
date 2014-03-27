namespace CarbonKnown.WCF.DataSource
{
    public enum SourceErrorType
    {
        GenericError = 0,
        DuplicateFile = 1,
        FileTypeNotFound = 2,
        UnReadableFile = 3,
        InvalidColumns = 4,
        ExceptionOccured = 5,
        MissingFields = 6
    }
}
