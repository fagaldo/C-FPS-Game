/// <summary>
/// Interfejs kluczowy dla mechaniki stanu zapisu gry.
/// Jest on implementowany przez każdą klasę, która zapisuje swoje pola.
/// </summary>
public interface ISaveable
{
    object SaveState();
    void LoadState(object state);
}
