namespace DeviceService.Services;

public sealed class ZKEMService : IZKEMService, IAsyncDisposable
{
    private readonly DeviceSession _session = new();

    private bool _connected;

    //**********************************************************
    //
    //  NOTE:   Use 32bit dlls and paste in C:\Windows\SysWOW64
    //
    //**********************************************************
    public async Task<bool> ConnectAsync(
        string ipAddress,
        int port,
        int communicationKey)
    {
        if (_connected)
            await DisconnectAsync();

        _connected = await _session.ExecuteAsync(zk =>
        {
            zk.SetCommPassword(communicationKey);

            return zk.Connect_Net(ipAddress, port);
        });

        return _connected;
    }

    public async Task DisconnectAsync()
    {
        if (!_connected)
            return;

        await _session.ExecuteAsync(zk =>
        {
            zk.Disconnect();
            return true;
        });

        _connected = false;
    }

    //public async Task<DeviceInformation?> GetInformationAsync(
    //    int machineNumber)
    //{
    //    if (!_connected)
    //        return null;

    //    return await _session.ExecuteAsync(zk =>
    //    {
    //        string serial = "";
    //        string firmware = "";
    //        string platform = "";
    //        string mac = "";

    //        bool serialOk = zk.GetSerialNumber(
    //            machineNumber, out serial);

    //        zk.GetFirmwareVersion(
    //            machineNumber, ref firmware);

    //        zk.GetPlatform(
    //            machineNumber, ref platform);

    //        zk.GetDeviceMAC(
    //            machineNumber, ref mac);

    //        int year = 0, month = 0, day = 0;
    //        int hour = 0, minute = 0, second = 0;

    //        bool timeOk = zk.GetDeviceTime(
    //            machineNumber,
    //            ref year,
    //            ref month,
    //            ref day,
    //            ref hour,
    //            ref minute,
    //            ref second);

    //        DateTime? deviceTime = null;

    //        if (timeOk)
    //        {
    //            try
    //            {
    //                deviceTime = new DateTime(
    //                    year, month, day,
    //                    hour, minute, second);
    //            }
    //            catch (ArgumentOutOfRangeException)
    //            {
    //                // Ignore invalid device clock values.
    //            }
    //        }

    //        if (!serialOk)
    //            return null;

    //        return new DeviceInformation
    //        {
    //            SerialNumber = serial,
    //            FirmwareVersion = firmware,
    //            Platform = platform,
    //            MacAddress = mac,
    //            DeviceTime = deviceTime
    //        };
    //    });
    //}

    //public async Task<IReadOnlyList<AttendanceRecord>>
    //    GetAttendanceAsync(int machineNumber)
    //{
    //    if (!_connected)
    //        return [];

    //    return await _session.ExecuteAsync(zk =>
    //    {
    //        var records = new List<AttendanceRecord>();

    //        if (!zk.ReadGeneralLogData(machineNumber))
    //            return (IReadOnlyList<AttendanceRecord>)records;

    //        string enrollNumber = "";

    //        int verifyMode = 0;
    //        int inOutMode = 0;

    //        int year = 0, month = 0, day = 0;
    //        int hour = 0, minute = 0, second = 0;
    //        int workCode = 0;

    //        while (zk.SSR_GetGeneralLogData(
    //            machineNumber,
    //            out enrollNumber,
    //            out verifyMode,
    //            out inOutMode,
    //            out year,
    //            out month,
    //            out day,
    //            out hour,
    //            out minute,
    //            out second,
    //            ref workCode))
    //        {
    //            try
    //            {
    //                records.Add(new AttendanceRecord
    //                {
    //                    EnrollNumber = enrollNumber,
    //                    Timestamp = new DateTime(
    //                        year, month, day,
    //                        hour, minute, second),
    //                    VerifyMode = verifyMode,
    //                    InOutMode = inOutMode,
    //                    WorkCode = workCode
    //                });
    //            }
    //            catch (ArgumentOutOfRangeException)
    //            {
    //                // Skip malformed device records.
    //            }
    //        }

    //        return (IReadOnlyList<AttendanceRecord>)records;
    //    });
    //}

    //public async Task<IReadOnlyList<DeviceUser>>
    //    GetUsersAsync(int machineNumber)
    //{
    //    if (!_connected)
    //        return [];

    //    return await _session.ExecuteAsync(zk =>
    //    {
    //        var users = new List<DeviceUser>();

    //        if (!zk.ReadAllUserID(machineNumber))
    //            return (IReadOnlyList<DeviceUser>)users;

    //        string enrollNumber;
    //        string name;
    //        string password;
    //        int privilege;
    //        bool enabled;

    //        while (zk.SSR_GetAllUserInfo(
    //            machineNumber,
    //            out enrollNumber,
    //            out name,
    //            out password,
    //            out privilege,
    //            out enabled))
    //        {
    //            users.Add(new DeviceUser
    //            {
    //                EnrollNumber = enrollNumber,
    //                Name = name,
    //                Privilege = privilege,
    //                Enabled = enabled
    //            });
    //        }

    //        return (IReadOnlyList<DeviceUser>)users;
    //    });
    //}

    //public async Task<bool> SetUserAsync(
    //    int machineNumber,
    //    DeviceUser user)
    //{
    //    if (!_connected)
    //        return false;

    //    return await _session.ExecuteAsync(zk =>
    //        zk.SSR_SetUserInfo(
    //            machineNumber,
    //            user.EnrollNumber,
    //            user.Name,
    //            user.Password ?? "",
    //            user.Privilege,
    //            user.Enabled));
    //}

    //public async Task<bool> RegisterEventsAsync(
    //    int machineNumber)
    //{
    //    if (!_connected)
    //        return false;

    //    return await _session.ExecuteAsync(zk =>
    //        zk.RegEvent(machineNumber, 65535));
    //}

    public async ValueTask DisposeAsync()
    {
        if (_connected)
            await DisconnectAsync();

        await _session.DisposeAsync();
    }
}