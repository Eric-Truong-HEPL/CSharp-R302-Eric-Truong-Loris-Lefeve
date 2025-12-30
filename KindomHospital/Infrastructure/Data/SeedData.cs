using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KindomHospital.Domain.Entities;

namespace KindomHospital.Infrastructure.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Appliquer les migrations
            db.Database.Migrate();

            // Seed Specialties (simple ASCII names to avoid encoding issues)
            if (!db.Specialties.Any())
            {
                var specs = new List<Specialty>
                {
                    new Specialty { Name = "Chirurgie Generale", Category = "Chirurgicale", Description = "Interventions abdominales et digestives" },
                    new Specialty { Name = "Chirurgie Orthopedique", Category = "Chirurgicale", Description = "Os, articulations et ligaments" },
                    new Specialty { Name = "Chirurgie Cardiaque", Category = "Chirurgicale", Description = "Coeur et vaisseaux" },
                    new Specialty { Name = "Neurochirurgie", Category = "Chirurgicale", Description = "Cerveau, moelle epiniere et nerfs" },
                    new Specialty { Name = "Chirurgie Plastique", Category = "Chirurgicale", Description = "Reconstructrice et esthetique" },
                    new Specialty { Name = "Chirurgie Thoracique", Category = "Chirurgicale", Description = "Poumons et cage thoracique" },
                    new Specialty { Name = "Chirurgie Vasculaire", Category = "Chirurgicale", Description = "Arteres et veines" },
                    new Specialty { Name = "Chirurgie Pediatrique", Category = "Chirurgicale", Description = "Interventions chez l'enfant" },
                    new Specialty { Name = "Medecine Generale", Category = "Medicale", Description = "Soins primaires" },
                    new Specialty { Name = "Cardiologie", Category = "Medicale", Description = "Maladies cardiovasculaires" },
                    new Specialty { Name = "Pneumologie", Category = "Medicale", Description = "Maladies respiratoires" },
                    new Specialty { Name = "Gastro-enterologie", Category = "Medicale", Description = "Systeme digestif" },
                    new Specialty { Name = "Nephrologie", Category = "Medicale", Description = "Reins et voies urinaires" },
                    new Specialty { Name = "Endocrinologie", Category = "Medicale", Description = "Hormones et metabolisme" },
                    new Specialty { Name = "Rhumatologie", Category = "Medicale", Description = "Articulations, os et muscles" },
                    new Specialty { Name = "Neurologie", Category = "Medicale", Description = "Systeme nerveux" },
                    new Specialty { Name = "Hematologie", Category = "Medicale", Description = "Sang et organes hematopoietiques" },
                    new Specialty { Name = "Oncologie", Category = "Medicale", Description = "Cancers et tumeurs" },
                    new Specialty { Name = "Gynecologie", Category = "Femme et Enfant", Description = "Appareil genital feminin" },
                    new Specialty { Name = "Obstetrique", Category = "Femme et Enfant", Description = "Grossesse et accouchement" },
                    new Specialty { Name = "Pediatrie", Category = "Femme et Enfant", Description = "Medecine de l'enfant" },
                    new Specialty { Name = "Neonatologie", Category = "Femme et Enfant", Description = "Nouveaux-nes et prematures" },
                    new Specialty { Name = "Ophtalmologie", Category = "Organes des Sens", Description = "Yeux et vision" },
                    new Specialty { Name = "ORL", Category = "Organes des Sens", Description = "Oreilles, nez et gorge" },
                    new Specialty { Name = "Dermatologie", Category = "Organes des Sens", Description = "Peau, cheveux et ongles" },
                    new Specialty { Name = "Anesthesiologie", Category = "Transversale", Description = "Anesthesie et reanimation" },
                    new Specialty { Name = "Radiologie", Category = "Transversale", Description = "Imagerie medicale" },
                    new Specialty { Name = "Medecine Interne", Category = "Transversale", Description = "Maladies complexes multi-organes" },
                    new Specialty { Name = "Psychiatrie", Category = "Transversale", Description = "Sante mentale" },
                    new Specialty { Name = "Geriatrie", Category = "Transversale", Description = "Medecine des personnes agees" },
                    new Specialty { Name = "Medecine d'Urgence", Category = "Transversale", Description = "Urgences et SAMU" },
                    new Specialty { Name = "Medecine du Travail", Category = "Transversale", Description = "Sante au travail" },
                    new Specialty { Name = "Medecine Legale", Category = "Transversale", Description = "Expertise judiciaire" },
                    new Specialty { Name = "Allergologie", Category = "Medicale", Description = "Allergies et immunologie" },
                    new Specialty { Name = "Infectiologie", Category = "Medicale", Description = "Maladies infectieuses" },
                    new Specialty { Name = "Medecine Physique et Readaptation", Category = "Transversale", Description = "Reeducation fonctionnelle" },
                    new Specialty { Name = "Medecine Nucleaire", Category = "Transversale", Description = "Diagnostic et therapie par isotopes" },
                    new Specialty { Name = "Anatomo-pathologie", Category = "Transversale", Description = "Analyse tissulaire et diagnostic" },
                    new Specialty { Name = "Biologie Medicale", Category = "Transversale", Description = "Analyses de laboratoire" },
                    new Specialty { Name = "Addictologie", Category = "Transversale", Description = "Dependances et toxicomanies" }
                };

                // Trim names and enforce max lengths before insert
                foreach (var s in specs)
                {
                    s.Name = s.Name?.Trim();
                    if (s.Name != null && s.Name.Length > 30) s.Name = s.Name.Substring(0, 30);
                    if (s.Category != null && s.Category.Length > 50) s.Category = s.Category.Substring(0, 50);
                    if (s.Description != null && s.Description.Length > 255) s.Description = s.Description.Substring(0, 255);
                }
                db.Specialties.AddRange(specs);
                db.SaveChanges();
            }

            // Seed Medicaments
            if (!db.Medicaments.Any())
            {
                var meds = new List<Medicament>
                {
                    new Medicament { Name = "Paracetamol", DosageForm = "Comprime", Strength = "500mg", AtcCode = "N02BE01" },
                    new Medicament { Name = "Ibuprofene", DosageForm = "Comprime", Strength = "400mg", AtcCode = "M01AE01" },
                    new Medicament { Name = "Amoxicilline", DosageForm = "Gelule", Strength = "500mg", AtcCode = "J01CA04" },
                    new Medicament { Name = "Aspirine", DosageForm = "Comprime", Strength = "100mg", AtcCode = "N02BA01" },
                    new Medicament { Name = "Omeprazole", DosageForm = "Gelule", Strength = "20mg", AtcCode = "A02BC01" },
                    new Medicament { Name = "Metformine", DosageForm = "Comprime", Strength = "850mg", AtcCode = "A10BA02" },
                    new Medicament { Name = "Atorvastatine", DosageForm = "Comprime", Strength = "20mg", AtcCode = "C10AA05" },
                    new Medicament { Name = "Losartan", DosageForm = "Comprime", Strength = "50mg", AtcCode = "C09CA01" },
                    new Medicament { Name = "Amlodipine", DosageForm = "Comprime", Strength = "5mg", AtcCode = "C08CA01" },
                    new Medicament { Name = "Levothyroxine", DosageForm = "Comprime", Strength = "75mcg", AtcCode = "H03AA01" },
                    new Medicament { Name = "Clopidogrel", DosageForm = "Comprime", Strength = "75mg", AtcCode = "B01AC04" },
                    new Medicament { Name = "Metoprolol", DosageForm = "Comprime", Strength = "50mg", AtcCode = "C07AB02" },
                    new Medicament { Name = "Simvastatine", DosageForm = "Comprime", Strength = "40mg", AtcCode = "C10AA01" },
                    new Medicament { Name = "Tramadol", DosageForm = "Gelule", Strength = "50mg", AtcCode = "N02AX02" },
                    new Medicament { Name = "Salbutamol", DosageForm = "Inhalateur", Strength = "100mcg", AtcCode = "R03AC02" },
                    new Medicament { Name = "Prednisolone", DosageForm = "Comprime", Strength = "20mg", AtcCode = "H02AB06" },
                    new Medicament { Name = "Ciprofloxacine", DosageForm = "Comprime", Strength = "500mg", AtcCode = "J01MA02" },
                    new Medicament { Name = "Azithromycine", DosageForm = "Comprime", Strength = "250mg", AtcCode = "J01FA10" },
                    new Medicament { Name = "Furosemide", DosageForm = "Comprime", Strength = "40mg", AtcCode = "C03CA01" },
                    new Medicament { Name = "Warfarine", DosageForm = "Comprime", Strength = "5mg", AtcCode = "B01AA03" },
                    new Medicament { Name = "Diazepam", DosageForm = "Comprime", Strength = "10mg", AtcCode = "N05BA01" },
                    new Medicament { Name = "Fluoxetine", DosageForm = "Gelule", Strength = "20mg", AtcCode = "N06AB03" },
                    new Medicament { Name = "Sertraline", DosageForm = "Comprime", Strength = "50mg", AtcCode = "N06AB06" },
                    new Medicament { Name = "Insuline Glargine", DosageForm = "Injectable", Strength = "100UI/mL", AtcCode = "A10AE04" },
                    new Medicament { Name = "Ramipril", DosageForm = "Comprime", Strength = "5mg", AtcCode = "C09AA05" },
                    new Medicament { Name = "Spironolactone", DosageForm = "Comprime", Strength = "25mg", AtcCode = "C03DA01" },
                    new Medicament { Name = "Digoxine", DosageForm = "Comprime", Strength = "0.25mg", AtcCode = "C01AA05" },
                    new Medicament { Name = "Codeine", DosageForm = "Comprime", Strength = "30mg", AtcCode = "R05DA04" },
                    new Medicament { Name = "Morphine", DosageForm = "Injectable", Strength = "10mg/mL", AtcCode = "N02AA01" },
                    new Medicament { Name = "Heparine", DosageForm = "Injectable", Strength = "5000UI/mL", AtcCode = "B01AB01" }
                };

                db.Medicaments.AddRange(meds);
                db.SaveChanges();
            }

            // Ensure medicament names trimmed
            foreach (var m in db.Medicaments)
            {
                m.Name = m.Name?.Trim();
                m.DosageForm = m.DosageForm?.Trim();
                m.Strength = m.Strength?.Trim();
            }
            db.SaveChanges();

            // Seed Doctors
            if (!db.Doctors.Any())
            {
                var doctors = new List<Doctor>
                {
                    new Doctor { FirstName = "Eric", LastName = "Truong", SpecialtyId = db.Specialties.First(s => s.Name == "Medecine Generale").Id },
                    new Doctor { FirstName = "Loris", LastName = "Lefeve", SpecialtyId = db.Specialties.First(s => s.Name == "Cardiologie").Id },
                    new Doctor { FirstName = "Mathias", LastName = "Delhaze", SpecialtyId = db.Specialties.First(s => s.Name == "Dermatologie").Id },
                    new Doctor { FirstName = "Nathan", LastName = "Davin", SpecialtyId = db.Specialties.First(s => s.Name == "Neurologie").Id },
                    new Doctor { FirstName = "Jeremy", LastName = "Moreau", SpecialtyId = db.Specialties.First(s => s.Name == "Ophtalmologie").Id },
                    new Doctor { FirstName = "Amelie", LastName = "Petit", SpecialtyId = db.Specialties.First(s => s.Name == "Pediatrie").Id }
                };

                db.Doctors.AddRange(doctors);
                db.SaveChanges();
            }

            // Seed Patients
            if (!db.Patients.Any())
            {
                var patients = new List<Patient>
                {
                    new Patient { FirstName = "Francois", LastName = "Malmendier", BirthDate = new DateTime(1988, 4, 12) },
                    new Patient { FirstName = "Marc", LastName = "Leroy", BirthDate = new DateTime(1975, 9, 3) },
                    new Patient { FirstName = "Julie", LastName = "Roussel", BirthDate = new DateTime(1995, 1, 21) },
                    new Patient { FirstName = "Olivier", LastName = "Fournier", BirthDate = new DateTime(1958, 7, 2) },
                    new Patient { FirstName = "Emma", LastName = "Gonzalez", BirthDate = new DateTime(2010, 11, 30) }
                };

                db.Patients.AddRange(patients);
                db.SaveChanges();
            }

            // Seed Consultations (10), ensure one day with 2 consultations for same doctor
            if (!db.Consultations.Any())
            {
                var rnd = new Random(42);
                var doctors = db.Doctors.ToList();
                var patients = db.Patients.ToList();
                var baseDate = DateTime.Today.AddDays(1);

                var consultations = new List<Consultation>();

                var docWithTwo = doctors[0];
                consultations.Add(new Consultation { DoctorId = docWithTwo.Id, PatientId = patients[0].Id, Date = baseDate, Hour = new TimeSpan(9, 0, 0), Reason = "Controle" });
                consultations.Add(new Consultation { DoctorId = docWithTwo.Id, PatientId = patients[1].Id, Date = baseDate, Hour = new TimeSpan(10, 30, 0), Reason = "Douleur" });

                while (consultations.Count < 10)
                {
                    var d = doctors[rnd.Next(doctors.Count)];
                    var p = patients[rnd.Next(patients.Count)];
                    var date = baseDate.AddDays(rnd.Next(0, 7));
                    var hour = new TimeSpan(8 + rnd.Next(0, 9), rnd.Next(0, 2) * 30, 0);
                    consultations.Add(new Consultation { DoctorId = d.Id, PatientId = p.Id, Date = date, Hour = hour, Reason = "Suivi" });
                }

                db.Consultations.AddRange(consultations);
                db.SaveChanges();
            }

            // Seed Ordonnances (>=5) and lines
            if (!db.Ordonnances.Any())
            {
                var doctors = db.Doctors.ToList();
                var patients = db.Patients.ToList();
                var meds = db.Medicaments.ToList();
                var consults = db.Consultations.ToList();
                var ords = new List<Ordonnance>();

                var ord1 = new Ordonnance
                {
                    DoctorId = doctors[0].Id,
                    PatientId = patients[0].Id,
                    ConsultationId = consults.FirstOrDefault(c => c.PatientId == patients[0].Id)?.Id,
                    Date = DateTime.Today,
                    Notes = "Traitement court"
                };
                ord1.Lignes.Add(new OrdonnanceLigne { MedicamentId = meds[0].Id, Dosage = "500 mg", Frequency = "3 fois/jour", Duration = "7 jours", Quantity = 14 });
                ord1.Lignes.Add(new OrdonnanceLigne { MedicamentId = meds[1].Id, Dosage = "400 mg", Frequency = "si besoin", Duration = "5 jours", Quantity = 10 });
                ord1.Lignes.Add(new OrdonnanceLigne { MedicamentId = meds[2].Id, Dosage = "500 mg", Frequency = "2 fois/jour", Duration = "3 jours", Quantity = 6 });
                ords.Add(ord1);

                var ord2 = new Ordonnance
                {
                    DoctorId = doctors[1].Id,
                    PatientId = patients[0].Id,
                    ConsultationId = consults.FirstOrDefault(c => c.PatientId == patients[0].Id)?.Id,
                    Date = DateTime.Today,
                    Notes = "Controle cardio"
                };
                ord2.Lignes.Add(new OrdonnanceLigne { MedicamentId = meds[10].Id, Dosage = "75 mg", Frequency = "1 fois/jour", Duration = "30 jours", Quantity = 30 });
                ords.Add(ord2);

                var rnd2 = new Random(21);
                for (int i = 0; i < 3; i++)
                {
                    var d = doctors[rnd2.Next(doctors.Count)];
                    var p = patients[rnd2.Next(patients.Count)];
                    var ord = new Ordonnance { DoctorId = d.Id, PatientId = p.Id, Date = DateTime.Today.AddDays(-rnd2.Next(0, 10)), Notes = "Prescription" };
                    var count = 1 + rnd2.Next(0, 2);
                    for (int j = 0; j < count; j++)
                    {
                        var m = meds[rnd2.Next(meds.Count)];
                        ord.Lignes.Add(new OrdonnanceLigne { MedicamentId = m.Id, Dosage = "Standard", Frequency = "2 fois/jour", Duration = "7 jours", Quantity = 7 });
                    }
                    ords.Add(ord);
                }

                db.Ordonnances.AddRange(ords);
                db.SaveChanges();
            }
        }
    }
}
