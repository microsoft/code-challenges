namespace MyCompany.Visitors.Data.Repositories
{
    using MyCompany.Visitors.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The team repository implementation
    /// </summary>
    public class TeamRepository : ITeamRepository
    {
        private readonly MyCompanyContext _context;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="context">the context dependency</param>
        public TeamRepository(MyCompanyContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/>
        /// </summary>
        /// <param name="teamId"><see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/></returns>
        public async Task<Team> GetAsync(int teamId)
        {
            return await _context.Teams.FindAsync(teamId);
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/>
        /// </summary>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/></returns>
        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams
                .ToListAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/>
        /// </summary>
        /// <param name="team"><see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/></returns>
        public async Task<int> AddAsync(Team team)
        {
            if (team == null)
                throw new ArgumentNullException("team");

            _context.Teams
                .Add(team);

            await _context.SaveChangesAsync();

            return team.TeamId;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/>
        /// </summary>
        /// <param name="team"><see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/></param>
        public async Task UpdateAsync(Team team)
        {
            if (team == null)
                throw new ArgumentNullException("team");

            _context.Entry<Team>(team)
                .State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/>
        /// </summary>
        /// <param name="teamId"><see cref="MyCompany.Visitors.Data.Repositories.ITeamRepository"/></param>
        public async Task DeleteAsync(int teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);

            if (team != null)
            {
                _context.Teams
                    .Remove(team);

                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Dispose all resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose all resource
        /// </summary>
        /// <param name="disposing">Dispose managed resources check</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
