using System.Threading.Tasks;

namespace NetGoLynx.Services
{
    public interface IFilesystemService
    {
        Task<(bool result, string message)> BackupSqliteDatabase();
    }
}
