import { Route } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es';
import { RemoteEntryComponent } from './entry.component';
import { routes } from '../app.routes';

registerLocaleData(localeEs, 'es-ES');

export const remoteRoutes: Route[] = [
  { 
    path: '', 
    component: RemoteEntryComponent,
    providers: [
      provideHttpClient(),
      { provide: LOCALE_ID, useValue: 'es-ES' }
    ],
    children: routes
  },
];
