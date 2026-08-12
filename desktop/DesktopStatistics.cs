namespace DesktopUi.DesktopCore;

public sealed class DesktopStatistics
{
    private long _opens, _updates, _closes, _accepted, _rejected;
    private int _activeWindows, _nodeSlotCount;

    public long OpenCount => Interlocked.Read(ref _opens);
    public long UpdateCount => Interlocked.Read(ref _updates);
    public long CloseCount => Interlocked.Read(ref _closes);
    public long AcceptedUpdateCount => Interlocked.Read(ref _accepted);
    public long RejectedUpdateCount => Interlocked.Read(ref _rejected);
    public int ActiveWindowCount => Volatile.Read(ref _activeWindows);
    public int NodeSlotCount => Volatile.Read(ref _nodeSlotCount);

    public void RecordOpen() => Interlocked.Increment(ref _opens);
    public void RecordUpdate() => Interlocked.Increment(ref _updates);
    public void RecordClose() => Interlocked.Increment(ref _closes);
    public void RecordAccepted() => Interlocked.Increment(ref _accepted);
    public void RecordRejected() => Interlocked.Increment(ref _rejected);
    public void SetActiveWindows(int v) => Volatile.Write(ref _activeWindows, v);
    public void SetNodeSlotCount(int v) => Volatile.Write(ref _nodeSlotCount, v);

    public object Snapshot() => new
    {
        open_count = OpenCount,
        update_count = UpdateCount,
        close_count = CloseCount,
        accepted_update_count = AcceptedUpdateCount,
        rejected_update_count = RejectedUpdateCount,
        active_windows = ActiveWindowCount,
        node_window_slot_count = NodeSlotCount
    };
}
