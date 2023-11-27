import {Injectable} from "@angular/core";
import {
    HttpInterceptor,
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpResponse
} from "@angular/common/http";
import {Observable} from "rxjs";
    import {ConfigService} from "./config.service";
import {tap} from "rxjs/operators";

@Injectable()
export class HttpInterceptorService implements HttpInterceptor {
    constructor(private _config: ConfigService) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const setHeaders: any = {
            "Context-Type": "application/json",
            "Access-Control-Allow-Origin": "*",
            "Access-Control-Expose-Headers": "Content-Length",
            "X-Requested-With": "XMLHttpRequest",
            "Cache-control": "no-cache, must-revalidate"
        };

        req = req.clone({setHeaders});

        return next.handle(req).pipe(tap((event: HttpEvent<any>) => {
                if (event instanceof HttpResponse) {
                }
            },
            (err: any) => {
                alert("에러발생");
                if (err.status === 401) return;
            })
        );

    }
}
