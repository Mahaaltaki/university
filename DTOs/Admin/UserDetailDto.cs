using System.Collections.Generic;
using System;

namespace kalamon_University.DTOs.Admin;

public class UserDetailDto
{
	public Guid Id { get; set; } // ��� �� ApplicationUserId
	public string Email { get; set; }
	public string Name { get; set; } // ����� �� ���� ��� ������� �� ��� ������ ����
	public IList<string> Roles { get; set; } = new List<string>(); // ["Admin", "Professor", "Student"]
																   //�� �� ����� ������ ����������
	public bool EmailConfirmed { get; set; }
	//��� ����� ��� ������ (��� �� ���� ������).
	public DateTimeOffset? LockoutEnd { get; set; }
	//�� �������� ���� ������ �������
	public bool TwoFactorEnabled { get; set; }
	//��� ������� ����� ������ �������.
	public int AccessFailedCount { get; set; }

	// ������� ���� ������� (��� ��� �������� ������)
	public Guid? StudentProfileId { get; set; } // PK ����� Student
	public string? StudentIdNumber { get; set; }

	// ������� ���� �������� (��� ��� �������� �������)
	public Guid? professorId { get; set; } // PK ����� Professor

	public string? Specialization { get; set; }

	// ����� ����� �� ���� ���� ����� ������ ������ ������
	// public DateTime CreatedDate { get; set; } // ����� ����� ������
}