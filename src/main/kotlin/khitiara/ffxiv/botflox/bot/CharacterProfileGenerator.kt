package khitiara.ffxiv.botflox.bot

import khitiara.ffxiv.botflox.xivapi.CharacterInfo
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.springframework.stereotype.Service
import org.springframework.web.reactive.function.client.WebClient
import org.springframework.web.reactive.function.client.awaitBody
import java.awt.Color
import java.awt.Graphics2D
import java.awt.image.BufferedImage
import java.io.ByteArrayInputStream
import javax.imageio.ImageIO

@Service
class CharacterProfileGenerator(private val client: WebClient) {
    suspend fun generateProfileImage(characterInfo: CharacterInfo): BufferedImage {
        val image = BufferedImage(1310, 873, BufferedImage.TYPE_INT_ARGB)
        val portraitBytes: ByteArray = client.get().uri(characterInfo.portraitUri).retrieve().awaitBody()
        val portrait: BufferedImage = withContext(Dispatchers.IO) {
            ByteArrayInputStream(portraitBytes).use { ImageIO.read(it) }
        }
        (image.graphics as Graphics2D).run {
            background = Color.WHITE
            clearRect(0, 0, 1310, 873)
            drawImage(portrait, 0, 0, null)
        }
        return image
    }
}