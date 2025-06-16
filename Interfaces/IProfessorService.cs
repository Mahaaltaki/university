using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kalamon_University.Models.Entities;

namespace kalamon_University.Interfaces
{
    /// <summary>
    /// æÇÌåÉ áÎÏãÇÊ ÅÏÇÑÉ ÇáÃÓÇÊĞÉ¡ ÊæİÑ ÇáÚãáíÇÊ ÇáÃÓÇÓíÉ (CRUD).
    /// </summary>
    public interface IProfessorService
    {
        /// <summary>
        /// ÌáÈ ŞÇÆãÉ ÈÌãíÚ ÇáÃÓÇÊĞÉ ãÚ ÈíÇäÇÊ ÇáãÓÊÎÏã ÇáãÑÊÈØÉ Èåã.
        /// </summary>
        /// <returns>ŞÇÆãÉ ÊÍÊæí Úáì ÌãíÚ ÇáÃÓÇÊĞÉ.</returns>
        Task<IEnumerable<Professor>> GetAllAsync();

        /// <summary>
        /// ÇáÈÍË Úä ÃÓÊÇĞ ãÍÏÏ ÈÇÓÊÎÏÇã ÇáãÚÑİ ÇáÎÇÕ Èå (UserId).
        /// </summary>
        /// <param name="professorId">ÇáãÚÑİ ÇáİÑíÏ (Guid) ááÃÓÊÇĞ.</param>
        /// <returns>ßÇÆä ÇáÃÓÊÇĞ ÅĞÇ Êã ÇáÚËæÑ Úáíå (ãÚ ÇáãæÇÏ ÇáÊí íÏÑÓåÇ)¡ æÅáÇ ÓíÚíÏ null.</returns>
        Task<Professor?> GetByIdAsync(Guid professorId);

        /// <summary>
        /// ÅÖÇİÉ ÃÓÊÇĞ ÌÏíÏ Åáì ŞÇÚÏÉ ÇáÈíÇäÇÊ.
        /// </summary>
        /// <param name="professor">ßÇÆä ÇáÃÓÊÇĞ ÇáĞí íÍÊæí Úáì ÇáÈíÇäÇÊ ÇáÌÏíÏÉ.</param>
        /// <returns>ßÇÆä ÇáÃÓÊÇĞ ÈÚÏ ÅÖÇİÊå æÍİÙå.</returns>
        Task<Professor> AddAsync(Professor professor);

        /// <summary>
        /// ÊÚÏíá ÈíÇäÇÊ ÃÓÊÇĞ ãæÌæÏ.
        /// </summary>
        /// <param name="professor">ßÇÆä ÇáÃÓÊÇĞ ÇáĞí íÍÊæí Úáì ÇáÈíÇäÇÊ ÇáãÍÏËÉ.</param>
        /// <returns>Task íÔíÑ Åáì ÇßÊãÇá ÇáÚãáíÉ.</returns>
        Task UpdateAsync(Professor professor);

        /// <summary>
        /// ÍĞİ ÃÓÊÇĞ ãä ŞÇÚÏÉ ÇáÈíÇäÇÊ ÈäÇÁğ Úáì ãÚÑİå.
        /// </summary>
        /// <param name="professorId">ÇáãÚÑİ ÇáİÑíÏ (Guid) ááÃÓÊÇĞ ÇáãÑÇÏ ÍĞİå.</param>
        /// <returns>True ÅĞÇ ÊãÊ ÚãáíÉ ÇáÍĞİ ÈäÌÇÍ¡ æ False ÅĞÇ áã íÊã ÇáÚËæÑ Úáì ÇáÃÓÊÇĞ.</returns>
        Task<bool> DeleteAsync(Guid professorId);
    }
}