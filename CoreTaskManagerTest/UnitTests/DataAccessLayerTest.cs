using CoreTaskmanager.Utilities;
using CoreTaskManager.Model;
using CoreTaskManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CoreTaskManagerTest.UnitTests
{
    public class DataAccessLayerTest
    {

        [Fact]
        public void Wether_CorrectBehave_Filtering_WhenInputSearchQuery_UsingTestData1()
        {
            using (var db = new CoreTaskManagerContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                db.Progresses.AddRange(GetSeedingProgressesTestData1());
                db.SaveChanges();
                var expectedProgresses = db.Progresses.Where(d => d.Title == "a");
                var expectedProgresses2 = db.Progresses.Where(d => d.Title == "��");
                var expectedProgresses3 = db.Progresses.Where(d => d.Title == "��");
                var expectedProgresses4 = db.Progresses.Where(d => d.Title == "�e�X�g");
                var expectedProgresses5 = db.Progresses.Where(d => d.Genre == "�\�t�g�E�F�A");
                var expectedProgresses6 = db.Progresses.Where(d => d.Genre == "�\�t�g�E�F�A").Where(d => d.Title == "Web�J��");

                // Assert
                var actualProgresses = db.FilterUsingSearchStrings("", "a");
                Assert.Equal(
                    expectedProgresses2.OrderBy(x => x.Id),
                    actualProgresses.OrderBy(x => x.Id)
                    );
                var actualProgresses2 = db.FilterUsingSearchStrings("", "��");
                Assert.Equal(
                    expectedProgresses2.OrderBy(x => x.Id),
                    actualProgresses2.OrderBy(x => x.Id)
                    );
                var actualProgresses3 = db.FilterUsingSearchStrings("", "��");
                Assert.Equal(
                    expectedProgresses3.OrderBy(x => x.Id),
                    actualProgresses3.OrderBy(x => x.Id)
                    );
                var actualProgresses4 = db.FilterUsingSearchStrings("", "�e�X�g");
                Assert.Equal(
                    expectedProgresses4.OrderBy(x => x.Id),
                    actualProgresses4.OrderBy(x => x.Id)
                    );
                var actualProgresses5 = db.FilterUsingSearchStrings("�\�t�g�E�F�A", "");
                Assert.Equal(
                    expectedProgresses5.OrderBy(x => x.Id),
                    actualProgresses5.OrderBy(x => x.Id)
                    );
                var actualProgresses6 = db.FilterUsingSearchStrings("�\�t�g�E�F�A", "Web�J��");
                Assert.Equal(
                    expectedProgresses6.OrderBy(x => x.Id),
                    actualProgresses6.OrderBy(x => x.Id)
                    );

            }
        }

        [Fact]
        public void Wether_CorrectBehave_Paging_UsingTestData1()
        {
            using (var db = new CoreTaskManagerContext(Utilities.TestDbContextOptions()))
            {
                // Arrage
                db.Progresses.AddRange(GetSeedingProgressesTestData1());
                db.SaveChanges();
                var progresses = db.Progresses.AsQueryable();

                // Act
                var result = progresses.Paging(1, 1);

                // Assert
                Assert.IsAssignableFrom<IQueryable<Progress>>(result);

                Assert.Equal(6, progresses.Paging(1, 6).Count());
                Assert.Equal(4, progresses.Paging(1, 4).Count());
                Assert.Equal(1, progresses.Paging(1, 1).Count());

                Assert.ThrowsAny<ArgumentException>(() =>
                {
                    return progresses.Paging(-1, 1);
                });
                Assert.ThrowsAny<ArgumentException>(() =>
                {
                    return progresses.Paging(0, -1);
                });
                Assert.ThrowsAny<ArgumentException>(() =>
                {
                    return progresses.Paging(1, -1);
                });
            }
        }

        [Fact]
        public void Wether_CorrectBehave_GenerateGenre_UsingTestData1()
        {
            using (var db = new CoreTaskManagerContext(Utilities.TestDbContextOptions()))
            {
                // Arrage
                db.Progresses.AddRange(GetSeedingProgressesTestData1());
                db.SaveChanges();

                // Act
                var genre = db.GenerateGenreList();

                // Assert
                Assert.Equal(2, genre.Count());
                Assert.Equal(1, genre.Where(g => g == "�\�t�g�E�F�A").Count());
                Assert.Equal(1, genre.Where(g => g == "����").Count());
            }
        }

        [Fact]
        public void Wether_CorrectBehave_GenerateGenre_UsingNoData()
        {
            using (var db = new CoreTaskManagerContext(Utilities.TestDbContextOptions()))
            {
                // Act
                var genre = db.GenerateGenreList();

                // Assert
                Assert.Equal(0, genre.Count());
            }
        }

        // Dont change this method contents! Tests depedent on this Method.
        private List<Progress> GetSeedingProgressesTestData1()
        {
            return new List<Progress>
                {
                    new Progress
                    {
                        Id = 1,
                        Title = "�e�X�g",
                        Genre = "����"
                    },
                    new Progress
                    {
                        Id = 2,
                        Title = "�e�X�g",
                        Genre = "�\�t�g�E�F�A"
                    },
                    new Progress
                    {
                        Id = 3,
                        Title = "Web�J��",
                        Genre = "�\�t�g�E�F�A"
                    },
                    new Progress
                    {
                        Id = 4,
                        Title = "Web�J��",
                        Genre = "�\�t�g�E�F�A"
                    },
                    new Progress
                    {
                        Id = 5,
                        Title = "��",
                        Genre = "����"
                    },
                    new Progress
                    {
                        Id = 6,
                        Title = "Web�J��",
                        Genre = "�\�t�g�E�F�A"
                    }
                };
        }
    }
}
