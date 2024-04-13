import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/containers' },
  { path: 'containers', loadChildren: () => import('./pages/containers/containers.module').then(m => m.ContainersModule) },
  { path: 'ships', loadChildren: () => import('./pages/ships/ships.module').then(m => m.ShipsModule) },
  { path: 'trucks', loadChildren: () => import('./pages/trucks/trucks.module').then(m => m.TrucksModule) },
  { path: 'about', loadChildren: () => import('./pages/about/about.module').then(m => m.AboutModule) }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
