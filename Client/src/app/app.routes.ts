import { Routes } from '@angular/router';
import { PropertyList } from '../components/property-list/property-list';
import { PropertyDetails } from '../components/property-details/property-details';
import { Login } from '../components/login/login';
import { Register } from '../components/register/register';
import { Addproprty } from '../components/addProperty/addproprty/addproprty';
import { MyProperties } from '../components/my-properties/my-properties';
import { Home } from '../components/home/home';
import { Images } from '../components/images/images';
import { User } from '../components/user/user';
import { authGuard } from '../guards/auth-guard';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'property-list', component: PropertyList },
  { path: 'property-details/:id', component: PropertyDetails },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'addproperty', component: Addproprty, canActivate: [authGuard] },
  { path: 'add-property/edit/:id', component: Addproprty, canActivate: [authGuard] },
  { path: 'my-properties', component: MyProperties, canActivate: [authGuard] },
  { path: 'home', component: Home },
  { path: 'images', component: Images },
  { path: 'user', component: User, canActivate: [authGuard] }
];
