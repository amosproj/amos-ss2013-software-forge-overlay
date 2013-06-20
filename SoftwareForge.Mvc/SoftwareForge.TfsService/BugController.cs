﻿/*
 * Copyright (c) 2013 by Denis Bach, Marvin Kampf, Konstantin Tsysin, Taner Tunc, Florian Wittmann
 *
 * This file is part of the Software Forge Overlay rating application.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public
 * License along with this program. If not, see
 * <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using SoftwareForge.DbService;
using Project = SoftwareForge.Common.Models.Project;
using ForgeWorkItem = SoftwareForge.Common.Models.WorkItem;


namespace SoftwareForge.TfsService
{
    public class BugController
    {
        private TfsConfigurationServer _tfsConfigurationServer;

        /// <summary>
        /// Constructor of the tfsController.
        /// </summary>
        /// <param name="tfsUri">The uri of the tfs</param>
        /// <param name="connectionString">The connection String to the mssql-server holding the ProjectCollections</param>
        public BugController(Uri tfsUri, String connectionString)
        {

            _tfsConfigurationServer = new TfsConfigurationServer(tfsUri);
            _tfsConfigurationServer.Authenticate();
        }

        /// <summary>
        /// Bool if the tfs has authenticated.
        /// </summary>
        private bool HasAuthenticated
        {
            get { return _tfsConfigurationServer.HasAuthenticated; }
        }

        public List<ForgeWorkItem> GetWorkItems(Guid teamProjectGuid)
        {
            List<ForgeWorkItem> workItems = new List<ForgeWorkItem>();
            Project project = ProjectsDao.Get(teamProjectGuid);
            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            TfsTeamProjectCollection tpc = _tfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid);
            WorkItemStore store = tpc.GetService<WorkItemStore>();
            WorkItemCollection workItemCollection = store.Query(
             " SELECT [System.Id], [System.WorkItemType]," +
             " [System.State], [System.AssignedTo], [System.Title] " +
             " FROM WorkItems " +
             " WHERE [System.TeamProject] = '" + project.Name +
            "' ORDER BY [System.WorkItemType], [System.Id]");
            foreach (WorkItem workItem in workItemCollection)
            {
                workItems.Add(new ForgeWorkItem{Title = workItem.Title});
            }
            return workItems;
        }
    }
}
