import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../services/auth";

export const roleGuard : CanActivateFn = (route) =>{
    const authService = inject(AuthService);
    const router = inject(Router);

    const requiredRole = route.data['role'] as string | undefined;

    if (!requiredRole || authService.role()=== requiredRole){
        return true;
    }
    router.navigate(['/']);
        return false;
};