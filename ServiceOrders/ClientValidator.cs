namespace ServiceOrders;

public static class ClientValidator
{
    // ИНН: 10 цифр (организация) или 12 цифр (физическое лицо)
    public static bool IsValidInn(string inn)
    {
        if (string.IsNullOrWhiteSpace(inn)) return false;
        if (inn.Length != 10 && inn.Length != 12) return false;
        foreach (char ch in inn)
            if (!char.IsDigit(ch)) return false;
        return true;
    }

    // VIN: 17 символов (буквы и цифры)
    public static bool IsValidVin(string vin)
    {
        if (string.IsNullOrWhiteSpace(vin) || vin.Length != 17) return false;
        foreach (char ch in vin)
            if (!char.IsLetterOrDigit(ch)) return false;
        return true;
    }
}
