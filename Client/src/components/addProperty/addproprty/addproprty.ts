// addproprty.ts
import { Component, inject, OnInit, AfterViewInit, ElementRef, ViewChild, ChangeDetectorRef } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PropertyService } from '../../../services/PropertiesService/property-service';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';

declare var google: any;

@Component({
  selector: 'app-addproprty',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './addproprty.html',
  styleUrl: './addproprty.css'
})
export class Addproprty implements OnInit, AfterViewInit {
  @ViewChild('cityInput') cityInput!: ElementRef;
  @ViewChild('addressInput') addressInput!: ElementRef;
  cityAutocompleteInstance: any;
  addressAutocompleteInstance: any;
  propertyService = inject(PropertyService);
  http = inject(HttpClient)
  router = inject(Router);
  route = inject(ActivatedRoute);
  cdr = inject(ChangeDetectorRef);
  selectedFiles: File[] = [];
  imagePreviews: string[] = [];
  propertyForm: FormGroup;
  cityBounds: any = null;
  selectedFile: File | null = null;
  isEditMode: boolean = false;
  propertyId: number | null = null;
  ownerIdFromLocalStorage: number | null = null;

  constructor(private fb: FormBuilder) {
    this.propertyForm = this.fb.group({
      Property: this.fb.group({
        PropertyID: [0],
        OwnerID: [0],
        Title: ['', Validators.required],
        City: ['', Validators.required],
        Address: ['', Validators.required],
        PricePerNight: [0, [Validators.required, Validators.min(1)]],
        Capacity: [0, [Validators.required, Validators.min(1)]],
        IsAvailable: [true],
        Description: [''],
        ImageUrl: ['']
      })
    });
  }
  removeImage(index: number) {
    this.selectedFiles.splice(index, 1);
    this.imagePreviews.splice(index, 1);
  }
  handleFiles(files: FileList) {
    this.selectedFiles = Array.from(files);
    this.imagePreviews = [];
    Array.from(files).forEach(file => {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreviews.push(e.target.result);
      };
      reader.readAsDataURL(file);
    });
  }
  ngOnInit() {
    const savedId = localStorage.getItem('Id');
    console.log("####", savedId)
    if (!savedId) {
      alert("לא נמצא מזהה בעלים. התחברי שנית.");
      this.router.navigate(['/login']);
      return;
    }
    this.ownerIdFromLocalStorage = Number(savedId);
  }
  onSubmit() {
    const formData = new FormData();
    const f = this.propertyForm.get('Property')?.value;

    formData.append('Title', f.Title || "");
    formData.append('City', f.City || "");
    formData.append('Address', f.Address || "");
    formData.append('PricePerNight', f.PricePerNight?.toString() || "0");
    formData.append('Capacity', f.Capacity?.toString() || "0");
    formData.append('Description', f.Description || "ללא תיאור");
    formData.append('Id', this.ownerIdFromLocalStorage!.toString());

    this.selectedFiles.forEach(file => {
      formData.append('images', file);
    });
    this.propertyService.addPropertyWithImage(this.ownerIdFromLocalStorage!, formData)
      .subscribe({
        next: (response) => {
          alert("הנכס נוסף בהצלחה!");
          this.router.navigate(['/my-properties']);
        },
        error: (err) => {
          console.error("Server Error Details:", err.error);
          alert("שגיאה בהוספת נכס. בדקי את ה-Console.");
        }
      });
  }
  onCityBlur() {
    const val = this.cityInput.nativeElement.value;
    if (val) this.propertyForm.patchValue({ Property: { City: val } });
  }
  onAddressBlur() {
    const val = this.addressInput.nativeElement.value;
    if (val) this.propertyForm.patchValue({ Property: { Address: val } });
  }
  loadPropertyToForm(id: number) {
    this.propertyService.getByID(id).subscribe({
      next: (data: any) => {
        const propertyData = data.Property || data;
        this.propertyForm.patchValue({ Property: propertyData });
        this.cdr.detectChanges();
      }
    });
  }
  ngAfterViewInit() {
    this.initGoogleAutocompletes();
  }
  initGoogleAutocompletes() {
    if (!this.cityInput || !this.addressInput) return;

    this.cityAutocompleteInstance = new google.maps.places.Autocomplete(this.cityInput.nativeElement, {
      types: ['(cities)'],
      componentRestrictions: { country: 'il' }
    });
    this.cityAutocompleteInstance.addListener('place_changed', () => {
      const place = this.cityAutocompleteInstance.getPlace();
      if (place) this.updateCitySelection(place);
    });
    this.addressAutocompleteInstance = new google.maps.places.Autocomplete(this.addressInput.nativeElement, {
      types: ['address'],
      componentRestrictions: { country: 'il' },
      strictBounds: true
    });
    this.addressAutocompleteInstance.addListener('place_changed', () => {
      const place = this.addressAutocompleteInstance.getPlace();
      if (place) this.validateAndSetAddress(place);
    });
  }
  updateCitySelection(place: any) {
    const cityName = place.name;
    this.propertyForm.patchValue({ Property: { City: cityName, Address: '' } });
    this.cityInput.nativeElement.value = cityName;
    this.addressInput.nativeElement.value = '';
    if (place.geometry && place.geometry.viewport) {
      this.cityBounds = place.geometry.viewport;
      this.addressAutocompleteInstance.setBounds(this.cityBounds);
    }
    this.cdr.detectChanges();
  }
  validateAndSetAddress(place: any) {
    const fullAddress = place.formatted_address || place.name || '';
    if (fullAddress) {
      this.propertyForm.patchValue({ Property: { Address: fullAddress } });
      this.addressInput.nativeElement.value = fullAddress;
    }
    this.cdr.detectChanges();
  }
  onFileSelected(event: any) {
    this.selectedFiles = Array.from(event.target.files);
  }
  onDrop(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      this.selectedFiles = Array.from(event.dataTransfer.files);
    }
  }
}
