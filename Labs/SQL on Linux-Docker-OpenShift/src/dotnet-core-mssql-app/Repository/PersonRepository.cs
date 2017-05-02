#region Copyright ©2017, Click2Cloud Inc. - All Rights Reserved
/* ------------------------------------------------------------------- *
*                            Click2Cloud Inc.                          *
*                  Copyright ©2016 - All Rights reserved               *
*                                                                      *
* Apache 2.0 License                                                   *
* You may obtain a copy of the License at                              * 
* http://www.apache.org/licenses/LICENSE-2.0                           *
* Unless required by applicable law or agreed to in writing,           *
* software distributed under the License is distributed on an "AS IS"  *
* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express  *
* or implied. See the License for the specific language governing      *
* permissions and limitations under the License.                       *
*                                                                      *
* -------------------------------------------------------------------  */
#endregion Copyright ©2017, Click2Cloud Inc. - All Rights Reserved

using dotnet_core_mssql_app.Contexts;
using dotnet_core_mssql_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_core_mssql_app.Repository
{
    public class PersonRepository : IPersonRepository
    {
        MyContext _context;
        public PersonRepository(MyContext context)
        {
            _context = context;
        }
        static List<Person> ContactsList = new List<Person>();

        public void Add(Person item)
        {
            //ContactsList.Add(item);
            _context.Persons.Add(item);
            _context.SaveChanges();
        }

        public bool CheckValidUserKey(string reqkey)
        {
            throw new NotImplementedException();
        }

        public Person Find(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Person> GetAll()
        {
            List<Person> tempP = _context.Persons.ToList();
            List<Person> personReturn = new List<Person>();
            foreach (Person p in tempP)
            {
                if (string.IsNullOrEmpty(p.Picture)) { p.Picture = "/pics/no_picture.jpg"; }
                else if (!p.Picture.Contains("/pics/")) { p.Picture = "/pics/" + p.Picture; }
                personReturn.Add(p);
            }
            return personReturn;
        }

        public void Remove(string Id)
        {
            throw new NotImplementedException();
        }

        public void Update(Person item)
        {
            throw new NotImplementedException();
        }
    }

    public interface IPersonRepository
    {
        void Add(Person item);
        IEnumerable<Person> GetAll();
        Person Find(string key);
        void Remove(string Id);
        void Update(Person item);

        bool CheckValidUserKey(string reqkey);
    }
}
