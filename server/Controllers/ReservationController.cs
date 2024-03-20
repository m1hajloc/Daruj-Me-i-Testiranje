using Context;
using Models;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using Services;
namespace Controllers
{

    [Controller]
    [Route("api/Reservation")]
    public class ReservationController : ControllerBase
    {

        private IReservationService _service;

        public ReservationController(MongoDbContext _mongoDBContext)
        {
            this._service = new ReservationService(_mongoDBContext);
        }

        [HttpGet]
        [Route("GetReservationById")]
        public async Task<IActionResult> GetReservationById(String? id)
        {
            try
            {
                var a = await _service.Repository.GetReservationById(id);
                return Ok(a);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllReservations")]
        public async Task<IActionResult> GetAllReservations()
        {
            var a = await _service.Repository.GetAllReservations();
            return Ok(a);
        }
        [HttpDelete]
        [Route("DeleteReservation")]
        public async Task<IActionResult> DeleteReservation(string? id)
        {
            try
            {
                var existingReservation = await _service.Repository.GetReservationById(id);
                if (existingReservation == null)
                {
                    // If the reservation with the provided ID doesn't exist, return BadRequest
                    return BadRequest("Reservation not found.");
                }

                // If the reservation exists, proceed with deletion
                await _service.DeleteReservation(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("CreateReservation")]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            try
            {
                // Check if OwnerId is null
                if (string.IsNullOrEmpty(reservation.OwnerId))
                {
                    // If OwnerId is null or empty, return BadRequest with a specific error message
                    return BadRequest("OwnerId cannot be null or empty.");
                }

                // Attempt to create the reservation
                var createdReservation = await _service.CreateReservation(reservation);

                // If the creation is successful, return 200 OK with the created reservation
                return Ok(createdReservation);
            }
            catch (Exception e)
            {
                // If an exception occurs during reservation creation, return 400 Bad Request with the error message
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetReservationsByOwnerId")]
        public async Task<IActionResult> GetReservationsByOwnerId(String? id)
        {
            var a = await _service.Repository.GetReservationsByOwnerId(id);
            return Ok(a);
        }
        [HttpGet]
        [Route("GetReservationsByCustomerId")]
        public async Task<IActionResult> GetReservationsByCustomerId(String? id)
        {
            var a = await _service.Repository.GetReservationsByCustomerId(id);
            return Ok(a);
        }


    }
}