import { ApplicationConfig, provideBrowserGlobalErrorListeners, inject } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { provideApollo } from 'apollo-angular';
import { HttpLink } from 'apollo-angular/http';
import { InMemoryCache } from '@apollo/client';
import { GraphQLEmployeeService } from './core/services/graphql-employee.service';
import { EmployeeDataService } from './core/services/employee-data.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    {
      provide: EmployeeDataService,
      useClass: GraphQLEmployeeService,
    },
    provideHttpClient(),
    provideApollo(() => {
      const httpLink = inject(HttpLink);

      return {
        link: httpLink.create({
          uri: 'https://localhost:7266/graphql',
        }),
        cache: new InMemoryCache(),
      };
    }),
  ],
};
