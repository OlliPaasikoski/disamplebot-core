using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicSampleBot.DocumentDBCore;
using BasicSampleBot.DocumentDBCore.Models;
using Microsoft.Azure.Documents;
using System.Threading.Tasks;

namespace DocumentDBCoreTests
{
    [TestClass]
    public class DocumentDBTests
    {

        private Guid _testGuid = Guid.Parse("c1d59e11-881f-41fc-a0b2-5e71cb0eb59d");

        [TestMethod]
        public void ConnectionToDocumentDBSucceeds()
        {
            // relies on App.config values
            try
            {
                DocumentDBRepository<TodoItem>.Initialize();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message, ex);
            }
        }

        [TestMethod]
        public async Task CreateDocumentTodoItemDoesNotFail()
        {
            var doc = await DocumentDBRepository<TodoItem>.CreateTodoItemIfNotExistsAsync(
                new TodoItem
                {
                    Id = _testGuid,
                    Name = "Test item",
                    Description = $"Created { DateTime.Now.Date }, can be removed",
                    Completed = false
                }
            );

            Assert.IsNotNull(doc);
        }

        [TestMethod]
        public async Task GetDocumentByIdReturnsItemWithCorrectIdAndContents()
        {
            Document item = await DocumentDBRepository<TodoItem>.GetTodoItemById(_testGuid);
            Assert.AreEqual(item.Id, _testGuid.ToString());
        }

        [TestMethod]
        public async Task GetDocumentByDocumentLinkReturnsItemWithCorrectIdAndContents()
        {
            var doc = await DocumentDBRepository<TodoItem>.GetTodoItemById(_testGuid);
            Document item = await DocumentDBRepository<TodoItem>.GetByDocumentLink(doc.SelfLink);
            Assert.AreEqual(item.Id, _testGuid.ToString());
        }

        [TestMethod]
        public async Task DeleteDocumentWithIdAsParameterSucceeds()
        {
            await DocumentDBRepository<TodoItem>.DeleteDocument(_testGuid);
            Document item = await DocumentDBRepository<TodoItem>.GetTodoItemById(_testGuid);
            Assert.IsNull(item);
        }
    }
}
