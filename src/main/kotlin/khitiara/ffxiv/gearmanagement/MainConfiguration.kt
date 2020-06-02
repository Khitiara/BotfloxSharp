package khitiara.ffxiv.gearmanagement

import org.springframework.beans.factory.annotation.Value
import org.springframework.context.annotation.Configuration

@Configuration
class MainConfiguration {
    @Value("\${botflox.token}")
    val botToken: String? = null
}