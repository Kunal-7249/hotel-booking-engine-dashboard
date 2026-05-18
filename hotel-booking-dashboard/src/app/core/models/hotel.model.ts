export interface Hotel {
    hotelId:number;
    name:string;
    city:string;
    starRating:number;
    pricePerNight:number;
}

export interface HotelDetail extends Hotel {
    description:string;
    isAvailable:boolean;
}