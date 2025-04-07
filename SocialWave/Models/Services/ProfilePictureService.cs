﻿using Microsoft.EntityFrameworkCore;
using SocialWave.Data;
using SocialWave.Exceptions;
using SocialWave.Models.ConcreteClasses;
using SocialWave.Models.Interfaces;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;

namespace SocialWave.Models.Services
{
    /// <summary>
    /// Service responsible for managing profile pictures.
    /// </summary>
    public class ProfilePictureService : IProfilePictureService
    {
        private readonly ILogger<ProfilePictureService> _logger;
        private readonly ApplicationDbContext _context;
        private const int MAX_IMAGE_SIZE_BYTES = 5 * 1024 * 1024;

        /// <param name="logger">The logger to log information, warnings, and errors.</param>
        public ProfilePictureService(ILogger<ProfilePictureService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        /// <summary>
        /// Adds a profile picture to the user asynchronously.
        /// </summary>
        /// <param name="imageBytes">The byte representation of the image.</param>
        /// <param name="user">The user to whom the picture is to be added.</param>
        /// <exception cref="UserException">Thrown if there is an error adding the picture.</exception>
        public async Task AddPictureProfileAsync(byte[] imageBytes, User user)
        {
            try
            {
                if (imageBytes == null || user == null)
                {
                    throw new UserException("An error occurred with a null reference!");
                }

                if (imageBytes.Length > MAX_IMAGE_SIZE_BYTES)
                {
                    throw new UserException("Image size exceeds the maximum allowed size.");
                }

                user.PictureProfile = imageBytes;
                _context.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully changed profile picture");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing profile photo");
                throw; // Важно пробросить исключение дальше
            }
        }

        /// <summary>
        /// Saves the profile picture asynchronously if it doesn't already exist.
        /// </summary>
        /// <param name="pictureData">The byte array representing the picture data.</param>
        /// <param name="webRootPath">The root path of the web application.</param>
        /// <returns>The URL of the saved profile picture.</returns>
        public async Task<string> SaveProfilePictureAsync(byte[] pictureData, string webRootPath)
        {
            try
            {
                string profilePictureUrl = string.Empty;
                string profilePicturesDirectory = Path.Combine(webRootPath, "profile-pictures");

                // Create the profile pictures directory if it doesn't exist
                if (!Directory.Exists(profilePicturesDirectory))
                {
                    Directory.CreateDirectory(profilePicturesDirectory);
                }

                string fileExtension = ".jpg";

                // Determine the image format based on the provided picture data
                if (pictureData != null)
                {
                    if (IsWebPImage(pictureData))
                    {
                        fileExtension = ".webp";
                    }
                    else if (IsJpegImage(pictureData))
                    {
                        fileExtension = ".jpeg";
                    }
                    else if (IsPngImage(pictureData))
                    {
                        fileExtension = ".png";
                    }

                    // Generate a unique file name for the profile picture
                    string fileName = GetFileHash(pictureData) + fileExtension;

                    // Construct the file path where the profile picture will be saved
                    string filePath = Path.Combine(profilePicturesDirectory, fileName);

                    // Check if the file already exists
                    if (!File.Exists(filePath))
                    {
                        // Write the picture data to the file asynchronously
                        await File.WriteAllBytesAsync(filePath, pictureData);

                    }
                    // Set the profile picture URL
                    profilePictureUrl = $"/profile-pictures/{fileName}";
                }

                _logger.LogInformation("Successful process of saving the profile picture on the server");
                return profilePictureUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in saving the profile picture on the server");
                throw;
            }
        }

        // Method to calculate the hash of the picture data
        private string GetFileHash(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(data);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// Checks if the provided byte array represents a WebP image.
        /// </summary>
        private bool IsWebPImage(byte[] imageData)
        {
            return imageData.Length > 12 &&
                   imageData[0] == 0x52 &&
                   imageData[1] == 0x49 &&
                   imageData[2] == 0x46 &&
                   imageData[3] == 0x46 &&
                   imageData[8] == 0x57 &&
                   imageData[9] == 0x45 &&
                   imageData[10] == 0x42 &&
                   imageData[11] == 0x50;
        }

        /// <summary>
        /// Checks if the provided byte array represents a JPEG image.
        /// </summary>
        private bool IsJpegImage(byte[] imageData)
        {
            return imageData.Length > 3 &&
                   imageData[0] == 0xFF &&
                   imageData[1] == 0xD8 &&
                   imageData[2] == 0xFF;
        }

        /// <summary>
        /// Checks if the provided byte array represents a PNG image.
        /// </summary>
        private bool IsPngImage(byte[] imageData)
        {
            return imageData.Length > 4 &&
                   imageData[0] == 0x89 &&
                   imageData[1] == 0x50 &&
                   imageData[2] == 0x4E &&
                   imageData[3] == 0x47;
        }
    }
}
