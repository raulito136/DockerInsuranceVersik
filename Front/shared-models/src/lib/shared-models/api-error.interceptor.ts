import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const apiErrorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessages: string[] = [];

      // 1. Manejo de errores de conexión/red (Status 0)
      if (error.status === 0) {
        // Aquí capturamos el net::ERR_CONNECTION_REFUSED y similares
        errorMessages = [`Error de red o conexión: ${error.message}`];
      } 
      // 2. Errores con estructura de API (envelope)
      else if (error.error && Array.isArray(error.error.errors)) {
        errorMessages = error.error.errors.map((err: any) => 
          err.field ? `${err.field}: ${err.message}` : err.message
        );
      } 
      // 3. Errores con mensaje simple del backend
      else if (error.error && error.error.message) {
        errorMessages = [error.error.message];
      } 
      // 4. Fallback para otros errores (404, 500 sin body, etc.)
      else {
        errorMessages = [`Error ${error.status}: ${error.statusText || 'Ha ocurrido un error inesperado.'}`];
      }

      // Propagamos el arreglo de strings al componente
      return throwError(() => errorMessages);
    })
  );
};