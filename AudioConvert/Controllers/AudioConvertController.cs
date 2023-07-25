using AudioConvert.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xabe.FFmpeg;

namespace AudioConvert.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AudioConvertController : ControllerBase
    {
        [HttpPost("acc2m4a")]
        public async Task<ActionResult<AudioConvertOutPutModel>> Post([FromBody] AudioConvertModel base64AudioObject)
        {
            var audioBytes = Convert.FromBase64String(base64AudioObject.accBase64Data);

            var aacFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".aac");
            await System.IO.File.WriteAllBytesAsync(aacFilePath, audioBytes);

            var m4aFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".m4a");

            IConversion conversion = await FFmpeg.Conversions.FromSnippet.Convert(aacFilePath, m4aFilePath);
            IConversionResult result = await conversion.Start();

            var m4aFileBytes = await System.IO.File.ReadAllBytesAsync(m4aFilePath);
            var m4aFileBase64 = Convert.ToBase64String(m4aFileBytes);

            // cleanup temp files
            System.IO.File.Delete(aacFilePath);
            System.IO.File.Delete(m4aFilePath);

            AudioConvertOutPutModel response = new AudioConvertOutPutModel()
            {
                m4aBase64Data = m4aFileBase64
            };
            return Ok(response);
        }
    }
}
