export interface IProperty {
  Id?: number;
  OwnerID?: number;
  Owner?: IOwner;
  Title?: string;
  City: string;
  Address: string;
  PricePerNight: number;
  Capacity: number;
  IsAvailable: boolean;
  Description: string;
  Amenities: IAmenity[];
  Reviews: IReview[];
  Images?: { ImageUrl: string }[];
  Latitude: number;
  Longitude: number;
}
export interface IOwner {
  Id: number;
  FullName?: string; 
  FirstName?: string;
  LastName?: string;
  PhoneNumber: string;
  Email: string;
  Password?: string;
}
export interface IAmenity {
  Name: string;
  Price: number;
}
export interface IReview {
  Id: number;
  PropertyId: number;
  Rating: number;
  Comment: string;
  Name: string;
  Date: string;
}
export interface IImages {
  Id: number;
  PropertyId: number;
  ImageUrl: string;
  AltText?: string;
  Property: IProperty;
}