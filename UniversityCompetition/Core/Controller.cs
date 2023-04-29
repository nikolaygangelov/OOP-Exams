using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversityCompetition.Core.Contracts;
using UniversityCompetition.Models;
using UniversityCompetition.Models.Contracts;
using UniversityCompetition.Repositories;
using UniversityCompetition.Utilities.Messages;

namespace UniversityCompetition.Core
{
    public class Controller : IController
    {
        private readonly SubjectRepository subjects;
        private readonly StudentRepository students;
        private readonly UniversityRepository universities;

        public Controller()
        {
            subjects = new SubjectRepository();
            students = new StudentRepository();
            universities = new UniversityRepository();
        }
        public string AddStudent(string firstName, string lastName)
        {
            if (students.FindByName($"{firstName} {lastName}") != null)
            {
                return String.Format(OutputMessages.AlreadyAddedStudent, firstName, lastName);
            }

            IStudent student = new Student(0, firstName, lastName);
            students.AddModel(student);
            return String.Format(OutputMessages.StudentAddedSuccessfully, firstName, lastName, students.GetType().Name);
        }

        public string AddSubject(string subjectName, string subjectType)
        {
            if (subjects.FindByName(subjectName) != null)
            {
                return String.Format(OutputMessages.AlreadyAddedSubject, subjectName);
            }

            ISubject subject;
            if (subjectType == "TechnicalSubject")
            {
                subject = new TechnicalSubject(0, subjectName);
            }
            else if (subjectType == "EconomicalSubject")
            {
                subject = new EconomicalSubject(0, subjectName);
            }
            else if (subjectType == "HumanitySubject")
            {
                subject = new HumanitySubject(0, subjectName);
            }
            else
            {
                return String.Format(OutputMessages.SubjectTypeNotSupported, subjectType);
            }

            subjects.AddModel(subject);

            return String.Format(OutputMessages.SubjectAddedSuccessfully, subjectType, subjectName, subjects.GetType().Name);

        }

        public string AddUniversity(string universityName, string category, int capacity, List<string> requiredSubjects)
        {
            if (universities.FindByName(universityName) != null)
            {
                return String.Format(OutputMessages.AlreadyAddedUniversity, universityName);
            }

            List<int> requiredSubjectsIds = requiredSubjects.Select(s => subjects.FindByName(s).Id).ToList();
            University university = new University(0, universityName, category, capacity, requiredSubjectsIds);
            universities.AddModel(university);
            return String.Format(OutputMessages.UniversityAddedSuccessfully, universityName, universities.GetType().Name);
        }

        public string ApplyToUniversity(string studentName, string universityName)
        {
            IStudent student = students.FindByName(studentName);
            string[] names = studentName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string firstName = names[0];
            string lastName = names[1];

            if (student == null)
            {
                return String.Format(OutputMessages.StudentNotRegitered, firstName, lastName);
            }

            IUniversity university = universities.FindByName(universityName);
            if (university == null)
            {
                return String.Format(OutputMessages.UniversityNotRegitered, universityName);
            }

            foreach (int id in university.RequiredSubjects)
            {
                if (!student.CoveredExams.Contains(id))
                {
                    return String.Format(OutputMessages.StudentHasToCoverExams, studentName, universityName);
                }
            }

            if (student.University == university)
            {
                return String.Format(OutputMessages.StudentAlreadyJoined, firstName, lastName, universityName);
            }

            student.JoinUniversity(university);
            return String.Format(OutputMessages.StudentSuccessfullyJoined, firstName, lastName, universityName);
        }

        public string TakeExam(int studentId, int subjectId)
        {
            IStudent student = students.FindById(studentId);
            if (student == null)
            {
                return String.Format(OutputMessages.InvalidStudentId);
            }

            ISubject subject = subjects.FindById(subjectId);

            if (subject == null)
            {
                return String.Format(OutputMessages.InvalidSubjectId);
            }

            if (student.CoveredExams.Contains(subjectId))
            {
                return String.Format(OutputMessages.StudentAlreadyCoveredThatExam, student.FirstName, student.LastName
                    , subject.Name);
            }

            student.CoverExam(subject);
            return String.Format(OutputMessages.StudentSuccessfullyCoveredExam, student.FirstName, student.LastName,
                subject.Name);
        }

        public string UniversityReport(int universityId)
        {
            IUniversity university = universities.FindById(universityId);
            StringBuilder sb = new StringBuilder();
            int studentsCount = students.Models.Where(s => s.University == university).Count();

            sb.AppendLine($"*** {university.Name} ***");
            sb.AppendLine($"Profile: {university.Category}");
            sb.AppendLine($"Students admitted: {studentsCount}");
            sb.AppendLine($"University vacancy: {university.Capacity - studentsCount}");

            return sb.ToString().TrimEnd();
        }
    }
}
