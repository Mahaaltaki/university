using System.Collections.Generic;
using System;

namespace kalamon_University.DTOs.Admin;

public class UserDetailDto
{
	public Guid Id { get; set; } // åĞÇ åæ ApplicationUserId
	public string Email { get; set; }
	public string Name { get; set; } // ÚÇÏÉğ ãÇ íßæä äİÓ ÇáÅíãíá Ãæ ÇÓã ãÓÊÎÏã İÑíÏ
	public IList<string> Roles { get; set; } = new List<string>(); // ["Admin", "Professor", "Student"]
																   //åá Êã ÊÃßíÏ ÇáÈÑíÏ ÇáÅáßÊÑæäí¿
	public bool EmailConfirmed { get; set; }
	//ãÊì íäÊåí Şİá ÇáÍÓÇÈ (ÅĞÇ Êã ÍÙÑå ãÄŞÊğÇ).
	public DateTimeOffset? LockoutEnd { get; set; }
	//åá ÇáãÓÊÎÏã İÚøá ÇáÊÍŞŞ ÈÎØæÊíä¿
	public bool TwoFactorEnabled { get; set; }
	//ÚÏÏ ãÍÇæáÇÊ ÊÓÌíá ÇáÏÎæá ÇáİÇÔáÉ.
	public int AccessFailedCount { get; set; }

	// ãÚáæãÇÊ ÎÇÕÉ ÈÇáØÇáÈ (ÅĞÇ ßÇä ÇáãÓÊÎÏã ØÇáÈğÇ)
	public Guid? StudentProfileId { get; set; } // PK áÌÏæá Student
	public string? StudentIdNumber { get; set; }

	// ãÚáæãÇÊ ÎÇÕÉ ÈÇáÏßÊæÑ (ÅĞÇ ßÇä ÇáãÓÊÎÏã ÏßÊæÑğÇ)
	public Guid? professorId { get; set; } // PK áÌÏæá Professor

	public string? Specialization { get; set; }

	// íãßäß ÅÖÇİÉ Ãí ÍŞæá ÃÎÑì ÊÑÇåÇ ãäÇÓÈÉ áÚÑÖåÇ ááÃÏãä
	// public DateTime CreatedDate { get; set; } // ÊÇÑíÎ ÅäÔÇÁ ÇáÍÓÇÈ
}