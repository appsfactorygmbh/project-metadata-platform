const themes = ['dark', 'light'];
themes.forEach((theme) => {
  describe('Generate README Screenshots - ' + theme, () => {
    //define all object mocks
    let apiTokens = [
      {
        id: 1,
        name: 'SCIM Token',
        scopes: ['SCIM'],
        expirationDate: '2027-04-30T14:22:01.578921+00:00',
      },
      {
        id: 3,
        name: 'Read Projects Token',
        scopes: [],
        expirationDate: '2027-04-3T24:12:01.578921+00:00',
      },
    ];
    let businessUnits = [
      {
        id: 1,
        businessUnitName: 'Finance',
      },
      {
        id: 3,
        businessUnitName: 'General',
      },
    ];
    let companies = [
      {
        id: 1,
        companyName: 'Appsfactory GmbH',
      },
      {
        id: 2,
        companyName: 'Appscompany GmbH',
      },
    ];
    let departments = [
      {
        id: 1,
        departmentName: 'Design',
      },
      {
        id: 3,
        departmentName: 'Consulting',
      },
    ];
    let officeLocations = [
      {
        id: 1,
        officeLocationName: 'Leipzig',
      },
      {
        id: 3,
        officeLocationName: 'München',
      },
    ];
    let teams = [
      {
        id: 1,
        teamName: 'Team 1',
        businessUnit: businessUnits[0],
        ptl: 'Max Mustermann',
      },
      {
        id: 8,
        teamName: 'Team 2',
        businessUnit: businessUnits[1],
        ptl: 'Jane Doe',
      },
    ];
    let projectPlugins = [
      {
        pluginName: 'Repository',
        url: 'https://info.cern.ch/hypertext/WWW/TheProject.html',
        displayName: 'Repository',
        id: 303,
      },
      {
        pluginName: 'Design',
        url: 'https://info.cern.ch/hypertext/WWW/TheProject.html',
        displayName: 'A Plugin',
        id: 301,
      },
    ];
    let globalPlugins = [
      {
        pluginName: 'Appstorage',
        id: 304,
        isArchived: false,
        keys: [],
        baseUrl: 'https://info.cern.ch/hypertext/WWW/TheProject.html',
      },
      {
        pluginName: 'Repository',
        id: 305,
        isArchived: false,
        keys: [],
        baseUrl: 'https://info.cern.ch/hypertext/WWW/TheProject.html',
      },
    ];
    let projects = [
      {
        id: 301,
        slug: 'app_project',
        projectName: 'App_Project',
        clientName: 'App Client',
        company: companies[0],
        isArchived: false,
        isEoC: true,
        companyState: 'INTERNAL',
        team: teams[0],
        ismsLevel: 'NORMAL',
        notes: 'These are notes',
      },
      {
        id: 303,
        slug: 'project_metadata_platform',
        projectName: 'Project Metadata Platform',
        clientName: 'Appsfactory GmbH',
        company: companies[0],
        isArchived: false,
        isEoC: false,
        ismsLevel: 'VERY_HIGH',
        notes: '',
      },
      {
        id: 304,
        slug: 'aiproject',
        projectName: 'AI Project',
        clientName: 'Mustercorp',
        company: companies[1],
        isArchived: false,
        isEoC: false,
        team: teams[1],
        ismsLevel: 'NORMAL',
        notes: '',
      },
    ];
    let users = [
      {
        schemas: [
          'urn:ietf:params:scim:schemas:core:2.0:User',
          'urn:ietf:params:scim:schemas:extension:enterprise:2.0:User',
          'urn:ietf:params:scim:schemas:extension:pmp:User',
        ],
        id: '1',
        externalId: '1',
        userName: 'admin@admin.admin',
        active: true,
        addresses: [],
        'urn:ietf:params:scim:schemas:extension:enterprise:2.0:User': {
          organization: companies[0].companyName,
        },
        'urn:ietf:params:scim:schemas:extension:pmp:User': {
          departments: [],
          teamSupport: [],
          jobTitles: [],
          team: [],
          businessUnits: [],
          isScimProvisioned: false,
        },
      },
      {
        schemas: [
          'urn:ietf:params:scim:schemas:core:2.0:User',
          'urn:ietf:params:scim:schemas:extension:enterprise:2.0:User',
          'urn:ietf:params:scim:schemas:extension:pmp:User',
        ],
        id: '999',
        externalId: '999',
        userName: 'max.mustermann@mustercorp.de',
        active: true,
        addresses: [{ locality: officeLocations[0].officeLocationName }],
        'urn:ietf:params:scim:schemas:extension:enterprise:2.0:User': {
          organization: companies[0].companyName,
        },
        'urn:ietf:params:scim:schemas:extension:pmp:User': {
          departments: departments.map((d) => d.departmentName),
          teamSupport: [],
          jobTitles: ['Design Team Lead'],
          team: teams.map((t) => t.teamName),
          businessUnits: businessUnits.map((b) => b.businessUnitName),
          isScimProvisioned: true,
        },
      },
    ];

    let logs = [
      {
        logMessage:
          'max.mustermann updated global plugin AzureDevops: set BaseUrl from https://pmp.appsfactory.app/ to https://pmp.appsfactory.app',
        timestamp: '2026-06-19T14:30:53+00:00',
      },
      {
        logMessage:
          'max.mustermann updated project AppsProject:  set IsEoC from False to True',
        timestamp: '2026-06-10T11:09:15+00:00',
      },
      {
        logMessage:
          'max.mustermann updated project TestProject:  set IsEoC from False to True',
        timestamp: '2026-06-10T11:08:09+00:00',
      },
      {
        logMessage:
          'max.mustermann updated project PMP:  set IsEoC from False to True',
        timestamp: '2026-06-10T11:03:15+00:00',
      },
      {
        logMessage:
          'jane.doe added a new plugin to project TestProject with properties: Plugin = Repository, DisplayName = Repo2, Url = localhost',
        timestamp: '2026-06-09T14:00:18+00:00',
      },
      {
        logMessage:
          'jane.doe added a new plugin to project TestProject with properties: Plugin = Repository, DisplayName = Repo1, Url = localhost',
        timestamp: '2026-06-09T14:00:18+00:00',
      },
      {
        logMessage:
          'jane.doe created a new project with properties: ProjectName = AppProject, Slug = app_project, ClientName = MusterCorp, OfferId = , Company = Appsfactory GmbH, CompanyState = EXTERNAL, IsmsLevel = NORMAL, Company = Team 200',
        timestamp: '2026-06-09T13:53:33+00:00',
      },
      {
        logMessage: 'admin removed user mustermann.max@mustercorp.de',
        timestamp: '2026-06-05T06:23:08+00:00',
      },
      {
        logMessage:
          'SCIM updated user max.mustermann: set OfficeLocation from null to Leipzig,  set Company from null to Appsfactory GmbH, set JobTitles from null to [Executive Chief of Chiefs], set Departments from null to [Design], set IsScimProvisioned from False to True',
        timestamp: '2026-06-04T14:28:04+00:00',
      },
      {
        logMessage:
          'SCIM added a new department with properties: DepartmentName = Financing',
        timestamp: '2026-06-04T14:28:04+00:00',
      },
      {
        logMessage:
          'SCIM added a new office location with properties: OfficeLocationName = Leipzig',
        timestamp: '2026-06-04T14:28:04+00:00',
      },
    ];

    beforeEach(() => {
      cy.viewport(1920, 1080);
      //define all mocked API calls
      cy.intercept('GET', '/Auth/refresh', {
        statusCode: 200,
        body: {
          accessToken: 'fake-jwt-token',
          refreshToken: 'fake-refresh-token',
        },
      }).as('refreshToken');

      cy.intercept('GET', '/Users/**', { statusCode: 200, body: users[1] }).as(
        'getUser',
      );
      cy.intercept('GET', '/Users', {
        statusCode: 200,
        body: {
          schemas: ['urn:ietf:params:scim:api:messages:2.0:ListResponse'],
          totalResults: 13,
          Resources: users,
        },
      }).as('getUsers');
      cy.intercept('GET', '/Users/me', { statusCode: 200, body: users[0] }).as(
        'getMe',
      );

      cy.intercept('GET', '/Companies', {
        statusCode: 200,
        body: companies,
      }).as('getCompanies');
      cy.intercept('GET', '/Companies/**', {
        statusCode: 200,
        body: companies[0],
      }).as('getCompany');

      cy.intercept('GET', '/BusinessUnits', {
        statusCode: 200,
        body: businessUnits,
      }).as('getBusinessUnits');
      cy.intercept('GET', '/BusinessUnits/**', {
        statusCode: 200,
        body: businessUnits[0],
      }).as('getBusinessUnit');

      cy.intercept('GET', '/Departments', {
        statusCode: 200,
        body: departments,
      }).as('getDepartments');
      cy.intercept('GET', '/Departments/**', {
        statusCode: 200,
        body: departments[0],
      }).as('getDepartment');

      cy.intercept('GET', '/OfficeLocations', {
        statusCode: 200,
        body: officeLocations,
      }).as('getOfficeLocations');
      cy.intercept('GET', '/OfficeLocations/**', {
        statusCode: 200,
        body: officeLocations[0],
      }).as('getOfficeLocation');

      cy.intercept('GET', '/Auth/ApiTokens', {
        statusCode: 200,
        body: apiTokens,
      }).as('getApiTokens');
      cy.intercept('GET', '/Auth/ApiTokens/**', {
        statusCode: 200,
        body: apiTokens[0],
      }).as('getApiToken');

      cy.intercept('GET', '/Teams', { statusCode: 200, body: teams }).as(
        'getTeams',
      );
      cy.intercept('GET', '/Teams/**', { statusCode: 200, body: teams[0] }).as(
        'getTeam',
      );

      cy.intercept('GET', '/Plugins', {
        statusCode: 200,
        body: globalPlugins,
      }).as('getPlugins');

      cy.intercept('GET', '/Projects', {
        statusCode: 200,
        body: projects,
      }).as('getProjects');
      cy.intercept('GET', '/Projects/*', {
        statusCode: 200,
        body: projects[0],
      }).as('getProject');
      cy.intercept('GET', '/Projects/**/plugins', {
        statusCode: 200,
        body: projectPlugins,
      }).as('getProjectPlugins');
      cy.intercept('GET', '/Projects/**/unarchivedPlugins', {
        statusCode: 200,
        body: projectPlugins,
      }).as('getUnarchivedProjectPlugins');
      cy.intercept('GET', '/Logs', {
        statusCode: 200,
        body: logs,
      }).as('getLogs');

      //set theme and fake auth token
      cy.window().then((win) => {
        win.localStorage.setItem('vueuse-color-scheme', theme);
        win.localStorage.setItem('auth_token', '"fake-jwt-token"');
      });
    });

    it('captures the login screen', () => {
      cy.window().then((win) => {
        win.localStorage.removeItem('auth_token');
      });
      cy.visit('/login');

      cy.get('form').should('be.visible');

      cy.screenshot('login-screen-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the project view', () => {
      cy.visit('/');
      cy.wait(200);
      cy.wait(['@getProjects', '@getTeams', '@getCompanies']);
      cy.screenshot('project-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the project information and edit view', () => {
      cy.visit(
        '/' + projects[0].slug + '?isEditing=false&projectId=' + projects[0].id,
      );
      cy.wait([
        '@getProject',
        '@getProjectPlugins',
        '@getUnarchivedProjectPlugins',
      ]);
      cy.screenshot('project-information-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
      cy.visit(
        '/' + projects[0].slug + '?isEditing=true&projectId=' + projects[0].id,
      );
      cy.wait([
        '@getProject',
        '@getProjectPlugins',
        '@getUnarchivedProjectPlugins',
      ]);
      cy.wait(200);
      cy.screenshot('project-information-edit-view-' + theme, {
        overwrite: true,
      });
    });

    it('captures the project creation view modal', () => {
      cy.visit('/');
      cy.wait(['@getProjects', '@getTeams', '@getCompanies']);

      cy.get('.anticon-plus').closest('button, .ant-float-btn').click();

      cy.get('.ant-modal').should('be.visible');
      cy.contains('.ant-modal-title', 'Create Project').click();
      cy.wait(200);

      cy.screenshot('create-project-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the user settings view', () => {
      cy.visit('/settings/user-management?userId=' + users[1].externalId);
      cy.wait(['@getUsers', '@getUser']);
      cy.wait(200);
      cy.screenshot('user-management-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the user creation view', () => {
      cy.visit('/settings/user-management/create');
      cy.wait([
        '@getUsers',
        '@getTeams',
        '@getCompanies',
        '@getDepartments',
        '@getOfficeLocations',
        '@getBusinessUnits',
      ]);
      cy.contains('.ant-modal-title', 'Create User').click();
      cy.wait(200);

      cy.screenshot('user-creation-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });
    it('captures the team settings view', () => {
      cy.visit('/settings/team-management?teamId=' + teams[0].id);
      cy.wait(['@getTeams', '@getTeam']);
      cy.wait(200);
      cy.screenshot('team-management-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the team creation view', () => {
      cy.visit('/settings/team-management/create');
      cy.wait(['@getTeams', '@getBusinessUnits']);
      cy.contains('.ant-modal-title', 'Create Team').click();
      cy.wait(200);

      cy.screenshot('team-creation-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the department settings view', () => {
      cy.visit(
        '/settings/department-management?departmentId=' + departments[0].id,
      );
      cy.wait(['@getDepartments', '@getDepartment']);
      cy.wait(200);
      cy.screenshot('department-management-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the department creation view', () => {
      cy.visit('/settings/department-management/create');
      cy.wait(['@getDepartments']);
      cy.contains('.ant-modal-title', 'Create Department').click();
      cy.wait(200);

      cy.screenshot('department-creation-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the company settings view', () => {
      cy.visit('/settings/company-management?companyId=' + companies[0].id);
      cy.wait(['@getCompanies', '@getCompany']);
      cy.wait(200);
      cy.screenshot('company-management-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the company creation view', () => {
      cy.visit('/settings/company-management/create');
      cy.wait(['@getCompanies']);
      cy.contains('.ant-modal-title', 'Create Company').click();
      cy.wait(200);

      cy.screenshot('company-creation-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the business unit settings view', () => {
      cy.visit(
        '/settings/business-unit-management?businessUnitId=' +
          businessUnits[0].id,
      );
      cy.wait(['@getBusinessUnits', '@getBusinessUnit']);
      cy.wait(200);
      cy.screenshot('business-unit-management-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the business unit creation view', () => {
      cy.visit('/settings/business-unit-management/create');
      cy.wait(['@getBusinessUnits']);
      cy.contains('.ant-modal-title', 'Create Business Unit').click();
      cy.wait(200);

      cy.screenshot('business-unit-creation-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the office location settings view', () => {
      cy.visit(
        '/settings/office-location-management?officeLocationId=' +
          officeLocations[0].id,
      );
      cy.wait(['@getOfficeLocations', '@getOfficeLocation']);
      cy.wait(200);
      cy.screenshot('office-location-management-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the office location creation view', () => {
      cy.visit('/settings/office-location-management/create');
      cy.wait(['@getOfficeLocations']);
      cy.contains('.ant-modal-title', 'Create Office Location').click();
      cy.wait(200);

      cy.screenshot('office-location-creation-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the token settings view', () => {
      cy.visit('/settings/api-token-management?tokenId=' + apiTokens[0].id);
      cy.wait(['@getApiTokens', '@getApiToken']);
      cy.wait(200);
      cy.screenshot('api-token-management-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the token creation view', () => {
      cy.visit('/settings/api-token-management/create');
      cy.wait(['@getApiTokens']);
      cy.contains('.ant-modal-title', 'Create API-Token').click();
      cy.wait(200);

      cy.screenshot('api-token-creation-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the global plugin settings view', () => {
      cy.visit('/settings/global-plugins');
      cy.wait(['@getPlugins']);
      cy.wait(200);
      cy.screenshot('global-plugin-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the global plugin creation view', () => {
      cy.visit('/settings/global-plugins/create');
      cy.wait(['@getPlugins']);
      cy.contains('.ant-modal-title', 'Create Plugin').click();
      cy.wait(200);

      cy.screenshot('global-plugin-creation-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the global plugin edit view', () => {
      cy.visit('/settings/global-plugins/edit?pluginId=' + globalPlugins[0].id);
      cy.wait(['@getPlugins']);
      cy.contains('.ant-modal-title', 'Edit Plugin').click();
      cy.wait(200);

      cy.screenshot('global-plugin-edit-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });

    it('captures the global logs view', () => {
      cy.visit('/settings/global-logs');
      cy.wait(['@getLogs']);

      cy.screenshot('global-logs-view-' + theme, {
        overwrite: true,
        capture: 'viewport',
       scale: true,
});
    });
  });
});
